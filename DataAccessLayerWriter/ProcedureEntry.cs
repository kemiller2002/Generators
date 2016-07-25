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
            return
                $@"
            if ({parameter.Name}.HasValue)
            {{
                {{
                    var parameter = new SqlParameter
                {{
                    Value = id,
                    ParameterName = ""{parameter.Name}"",
                }};

                    command.Parameters.Add(parameter);
                }}
            }}";
        }

        public static string ConvertToType(Field parameter)
        {
            var parameterType = Type.GetType($"System.{parameter.Type}");
            var outputType = (parameter.AllowsNull && !parameterType.IsClass) ? $"{parameter.Type}?" : parameter.Name;

            return $"{outputType} {parameter.Name}";
        }


        public static string Create(string procedureNamespace, string name, IEnumerable<Field> parameters)
        {

            var parameterEntries = parameters.Select(ConvertToType).Join(", ");

            var codeParameters = parameters.Select(CreateParameterEntry).Join(System.Environment.NewLine);

            return $@"
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Net.Configuration;


namespace {procedureNamespace}
{{

    public class {name}
    {{

        readonly SqlConnection _connection;

        public SelectEmployees(SqlConnection connection)
        {{
            _connection = connection;
        }}

        public IEnumerable<dynamic> Execute({parameterEntries})
        {{
            var command = new SqlCommand();
            command.Connection = _connection;
            command.CommandText = ""[{procedureNamespace}].[{name}]"";

            /*if (id.HasValue)
            {{
                var parameter = new SqlParameter
                {{
                    Value = id,
                    ParameterName = ""@id"",
                }};

                command.Parameters.Add(parameter);
            }}*/


            {codeParameters}


            var reader = command.ExecuteReader();

            while (reader.Read())
            {{
                dynamic record = new ExpandoObject();

                for (var ii = 0; ii < reader.FieldCount; ii++)
                {{
                    record[reader.GetName(ii)] = reader[ii];
                }}

                yield return record;
            }}
        }}


    }}

}}

";
    }






    }
}
