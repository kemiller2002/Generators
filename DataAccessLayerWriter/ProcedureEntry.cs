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

        public static string CreateResultEntry(ProcedureResult result)
        {

            var bindings = result.Columns.Select(c => $"{result.Name}").Join(System.Environment.NewLine);
            var properties = result.Columns.Select(c => $"public {c.TypeName} {c.Name}{{get;set;}}").Join(System.Environment.NewLine);

            return $@"
                namespace {result.SchemaName}
                {{
                    public class {result.Name}Result
                    {{
                        public {result.Name}Result (IDataReader reader)
                        {{
                            {bindings}
                        }}

                        {properties}

                    }}
                }}
            ";
        }


        public static string BuildResultExecutionCode(ProcedureResult result, string sqlCommandName)
        {

            if (result == null)
            {
                return $"{sqlCommandName}.Execute()";
            }

            return $@"
            var reader = command.ExecuteReader();

            while (reader.Read())
            {{
                yield return new {result.SchemaName}.{result.Name}Result(reader);
            }}
            ";

        }

        
        public static string Create(string procedureNamespace, string name, IEnumerable<Field> parameters, ProcedureResult result)
        {
            var parameterEntries = parameters.Select(ConvertToType).Join(", ");

            var codeParameters = parameters.Select(CreateParameterEntry).Join(System.Environment.NewLine);

            var resultCode = (result == null) ? String.Empty : CreateResultEntry(result);

            var executionResult = (result == null) ? "int" : $"IEnumerable<{result.SchemaName}.{result.Name}Result>";

            var executionCodeToCreateResult = BuildResultExecutionCode(result, "command");

            return $@"

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Net.Configuration;


{resultCode}

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
