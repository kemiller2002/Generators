using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayerWriter
{
    public static class ProcedureEntry
    {


        static string CreateParameterEntry(Field parameter)
        {

            var setParameterDirection = (parameter.IsOutput) ? ", Direction = ParameterDirection.InputOutput " : "";
            var parameterSize = parameter.Length ?? parameter.Type.SqlDefaultSize;

            var setParameterSize = $", Size = {parameterSize}";
            var setParameterType = $", SqlDbType =  SqlDbType.{parameter.Type.SqlDbType} ";
        

            var forceSetParameter = (parameter.IsOutput) ? $@"else {{
                    var parameter = new SqlParameter
                {{
                    ParameterName = ""{parameter.Name}""
                    {setParameterDirection}
                    {setParameterSize}
                    {setParameterType}
                }};

                command.Parameters.Add(parameter);
                

            }}" : ""  ;

            if (parameter.AllowsNull && !parameter.Type.IsClass)
            {

                return
                    $@"
            if ({parameter.Name.ToCamelCase()}.HasValue)
            {{
                var parameter = new SqlParameter
                {{
                    Value = {parameter.Name.ToCamelCase()},
                    ParameterName = ""{parameter.Name}""
                    {setParameterDirection}
                    {setParameterType}
                }};

                command.Parameters.Add(parameter);
                
            }}{forceSetParameter}";
            }


            var checkForNull = (parameter.Type.IsClass && !parameter.AllowsNull)
                ? $@" if({parameter.Name.ToCamelCase()} == null)
                    {{
                        throw new ArgumentException(""{parameter.Name} cannot be null"");
                    }}
                "
                : "";


            return
                $@"

            {checkForNull}
            if({parameter.Name.ToCamelCase()} != null)
            {{
                var {parameter.Name}Parameter = new SqlParameter
                {{
                     Value = {parameter
                        .Name.ToCamelCase()},
                    ParameterName = ""{parameter.Name}""
                    {setParameterDirection}
                    {setParameterType}
                }};
                
                command.Parameters.Add({parameter.Name}Parameter);
            }}
            {forceSetParameter}
            ";
        }


        public static string ConvertToType(Field parameter)
        {
            var outputType = (parameter.AllowsNull && !parameter.Type.IsClass) ? $"{parameter.Type.Name}?" : parameter.Type.Name;
            return $"{outputType} {parameter.Name.ToCamelCase()}";
        }

        public static string CreateRecord(ProcedureResult result, Dictionary<string, IType> types)
        {

            var bindings = result.Columns.Select(c => $"{c.Name.ToPascalCase()} = ({types[c.TypeName].Type.Name})reader[\"{c.Name}\"] ;").Join(System.Environment.NewLine);

            var properties = result
                                .Columns
                                .Select(c => $"public {types[c.TypeName].Type.Name} {c.Name.ToPascalCase()}{{get;set;}}")
                                                .Join(System.Environment.NewLine);
            var duplicateColumns =
                result.Columns.GroupBy(x => x.Name)
                    .Select(x => new {Key = x.Key, Count = x.Count()})
                    .Where(x => x.Count > 1).ToArray();

            var notes = (duplicateColumns.Any()) ? $@"
                    /*
                        The following columns have duplicate entries.  
                        You'll need to either alias a column or remove the duplicate name in the query.
                        
                        {duplicateColumns.Select(x=>x.Key).Join(", ")}

                    */
            " : "";


            return $@"
                namespace {result.SchemaName}
                {{
                    {notes}
                    public class {result.Name}Record
                    {{
                        public {result.Name}Record (IDataRecord reader)
                        {{
                            {bindings}
                        }}

                        {properties}

                    }}
                }}
            ";
        }


        public static string BuildResultExecutionCode(string procedureNamespace, string name, ProcedureResult result, string sqlCommandName, IEnumerable<Field> parameters)
        {
            var outputAssignment = parameters.Where(p => p.IsOutput)
                .Select(p => $"{p.Name.ToPascalCase()} = {p.Name}Parameter.Value;")
                .Join(System.Environment.NewLine);

            var setParameters = parameters.Where(p => p.IsOutput)
                .Select(p => $"{p.Name.ToPascalCase()} = ({p.Type.Name}){sqlCommandName}.Parameters[\"{p.Name}\"].Value");

            if (result == null)
            {

                var affectedRecordsParameters = setParameters.Union(new[] {"RecordsAffected = result"})
                    .Join("," + System.Environment.NewLine);

                return $@"var result = {sqlCommandName}.ExecuteNonQuery();
                    
                return new {procedureNamespace}.{name}Result
                {{
                    {affectedRecordsParameters}
                }};
                ";
            }


            var setParametersJoined = setParameters.Join("," + System.Environment.NewLine); 
            return $@"
            var reader = command.ExecuteReader();
            {setParametersJoined}

            return new {procedureNamespace}.{name}Result
                {{
                    {setParametersJoined}
                    Recordset = (from IDataRecord r in reader select new {procedureNamespace}.{name}Record  (r) )
                }};
            ";

        }

        public static string CreateResultClass(string procedureNamespace, string name, ProcedureResult result, IEnumerable<Field> parameters)
        {

            var executionResult = (result == null)
                ? "public int RecordsAffected {get;set;}" : $" public IEnumerable<{result.SchemaName}.{result.Name}Record> Recordset {{get;set;}}";

            var parameterResults = parameters.Where(p => p.IsOutput).Select(p => $"public {p.Type.Name} {p.Name.ToPascalCase()} {{get;set;}}").Join(System.Environment.NewLine);
            return $@"
                    namespace {procedureNamespace}
                    {{
                        public class {name}Result
                        {{
                            {executionResult}
                    
                            {parameterResults}
                        }}

                    }}
                ";
        }

        
        public static string Create(string procedureNamespace, string name, IEnumerable<Field> parameters, ProcedureResult result, Dictionary<string, IType> types)
        {
            var parameterEntries = parameters.Select(ConvertToType).Join(", ");

            var codeParameters = parameters.Select(CreateParameterEntry).Join(System.Environment.NewLine);

            var recordCode = (result == null) ? String.Empty : CreateRecord(result, types);

            var executionResult = $"{procedureNamespace}.{name}Result";

            var executionCodeToCreateResult = BuildResultExecutionCode(procedureNamespace, name, result, "command", parameters);

            var resultClass = CreateResultClass(procedureNamespace, name, result, parameters);


            return $@"

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using StructuredSight.Data;

{recordCode}

{resultClass}

namespace {procedureNamespace}
{{

    public class {name} : StructuredSight.Data.BaseAccess<{name}Result>
    {{

        public {name}(SqlConnection connection) : base(connection)
        {{
        }}

        public {executionResult} Execute({parameterEntries})
        {{
            var command = Connection.CreateCommand();
            command.CommandText = ""[{procedureNamespace}].[{name}]"";
            command.CommandType = CommandType.StoredProcedure;


            {codeParameters}


            {executionCodeToCreateResult}
        }}

    }}

}}

";
    }






    }
}
