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
            var outVariable = (parameter.AllowsNull)? "ref" : "" ;
            return $"{outVariable} {outputType} {parameter.Name}";
        }

        public static string CreateResultEntry(ProcedureResult result, Dictionary<string, IType> types)
        {

            var bindings = result.Columns.Select(c => $"{c.Name} = ({types[c.TypeName].Type.Name})reader[\"{c.Name}\"] ;").Join(System.Environment.NewLine);
            var properties = result.Columns.Select(c => $"public {types[c.TypeName].Type.Name} {c.Name}{{get;set;}}").Join(System.Environment.NewLine);

            return $@"
                namespace {result.SchemaName}
                {{
                    public class {result.Name}ResultEntry
                    {{
                        public {result.Name}ResultEntry (IDataReader reader)
                        {{
                            {bindings}
                        }}

                        {properties}

                    }}
                }}
            ";
        }


        public static string BuildResultExecutionCode(ProcedureResult result, string sqlCommandName, IEnumerable<Field> parameters)
        {
            var outputAssignment = parameters.Where(p => p.IsOutput)
                .Select(p => $"{p.Name} = {p.Name}Parameter.Value;")
                .Join(System.Environment.NewLine);


            if (result == null)
            {
                return $@"var result = {sqlCommandName}.Execute()

                    {outputAssignment}
                return result;
";
            }



            return $@"
            var reader = command.ExecuteReader();
            {outputAssignment}
            while (reader.Read())
            {{
                yield return new {result.SchemaName}.{result.Name}Result(reader);
            }}
            ";

        }

        public static string CreateResultClass(string procedureNamespace, string name, ProcedureResult result, IEnumerable<Field> parameters)
        {

            var executionResult = (result == null)
                ? "int RecordsAffected {get;set;}" : $" public {result.SchemaName}.{result.Name} Recordset {{get;set;}}";

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

            var resultEntryCode = (result == null) ? String.Empty : CreateResultEntry(result, types);

            var executionResult = (result == null) ? "int" : $"IEnumerable<{result.SchemaName}.{result.Name}Result>";

            var executionCodeToCreateResult = BuildResultExecutionCode(result, "command", parameters);

            var resultClass = CreateResultClass(procedureNamespace, name, result, parameters);


            return $@"

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Net.Configuration;


{resultEntryCode}

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
