

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Net.Configuration;


namespace dbo
{

    public class SelectUsers
    {

        readonly SqlConnection _connection;

        public SelectUsers(SqlConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<dynamic> Execute(Int32? @Id, String @FirstName)
        {
            var command = new SqlCommand
            {
                Connection = _connection,
                CommandText = "[dbo].[SelectUsers]"
            };

            
            if (@Id.HasValue)
            {
                var parameter = new SqlParameter
                {
                    Value = @Id,
                    ParameterName = "@Id",
                };

                command.Parameters.Add(parameter);
                
            }

            if(@FirstName == null)
            {
                throw new ArgumentException ("@FirstName cannot be null");
            }
            
            var @FirstNameParameter = new SqlParameter
            {
                 Value = @FirstName,
                ParameterName = "@FirstName",
            };

            command.Parameters.Add(@FirstNameParameter);

                    


            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                dynamic record = new ExpandoObject();

                for (var ii = 0; ii < reader.FieldCount; ii++)
                {
                    record[reader.GetName(ii)] = reader[ii];
                }

                yield return record;
            }
        }


    }

}

