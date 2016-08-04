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
            if (parameter.AllowsNull)
            {
                if (parameter.Type.IsClass)
                {
                    return
                        $@"
                        if({parameter.Name} != null)
                        {{
                            var parameter = new SqlParameter
                            {{
                                Value = {parameter.Name},
                                ParameterName = ""{parameter
                            .Name}"",
                            }};

                            command.Parameters.Add(parameter);
                        }}

                    ";
                }
                else
                {
                    return
                        $@"
            if ({parameter.Name}.HasValue)
            {{
                var parameter = new SqlParameter
                {{
                    Value = {parameter.Name},
                    ParameterName = ""{parameter
                            .Name}"",
                }};

                command.Parameters.Add(parameter);
                
            }}";
                }

            }
            else
            {

                if (parameter.Type.IsClass)
                {
                    return
                        $@"
            if({parameter.Name} == null)
            {{
                throw new ArgumentException (""{parameter
                .Name} cannot be null"");
            }}
            
            var {parameter.Name}Parameter = new SqlParameter
            {{
                 Value = {parameter.Name},
                ParameterName = ""{parameter
                    .Name}"",
            }};

            command.Parameters.Add({parameter.Name}Parameter);

                    ";
                }
                else
                {
                    return
                        $@"
                var parameter = new SqlParameter
                {{
                    Value = id,
                    ParameterName = ""{parameter
                            .Name}"",
                }};

                    command.Parameters.Add(parameter);
                ";
                }



            }
        }

        public static string ConvertToType(Field parameter)
        {
            var outputType = (parameter.AllowsNull && !parameter.Type.IsClass) ? $"{parameter.Type.Name}?" : parameter.Type.Name;
            return $"{outputType} {parameter.Name}";
        }

        public static string CreateRecord(ProcedureResult result, Dictionary<string, IType> types)
        {

            var bindings = result.Columns.Select(c => $"{c.Name} = ({types[c.TypeName].Type.Name})reader[\"{c.Name}\"] ;").Join(System.Environment.NewLine);
            var properties = result.Columns.Select(c => $"public {types[c.TypeName].Type.Name} {c.Name}{{get;set;}}").Join(System.Environment.NewLine);

            return $@"
                namespace {result.SchemaName}
                {{
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
                .Select(p => $"{p.Name} = {p.Name}Parameter.Value;")
                .Join(System.Environment.NewLine);

            var setParameters = parameters.Where(p => p.IsOutput)
                .Select(p => $"{p.Name} = {sqlCommandName}.{p.Name}.Value");

            if (result == null)
            {

                var affectedRecordsParameters = setParameters.Union(new[] {"RecordsAffected = result"})
                    .Join("," + System.Environment.NewLine);

                return $@"var result = {sqlCommandName}.Execute()
                    
                return new {procedureNamespace}{name}Result
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
                ? "int RecordsAffected {get;set;}" : $" public IEnumerable<{result.SchemaName}.{result.Name}Record> Recordset {{get;set;}}";

            var parameterResults = parameters.Where(p => p.IsOutput).Select(p => $"public {p.Type.Name} {p.Name} {{get;set;}}").Join(System.Environment.NewLine);
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
using System.Dynamic;
using System.Net.Configuration;
using System.Linq;


{recordCode}

{resultClass}

namespace {procedureNamespace}
{{

    public class {name}
    {{

        readonly SqlConnection _connection;

        public {name}(SqlConnection connection)
        {{
            _connection = connection;
        }}

        public {executionResult} Execute({parameterEntries})
        {{
            var command = new SqlCommand
            {{
                Connection = _connection,
                CommandText = ""[{procedureNamespace}].[{name}]""
            }};

            {codeParameters}


            {executionCodeToCreateResult}
        }}


    }}

}}

";
    }






    }
}
