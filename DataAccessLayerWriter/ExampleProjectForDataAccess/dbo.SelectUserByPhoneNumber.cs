

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Net.Configuration;


namespace dbo
{

    public class SelectUserByPhoneNumber
    {

        readonly SqlConnection _connection;

        public SelectUserByPhoneNumber(SqlConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<dynamic> Execute(String @PhoneNumber)
        {
            var command = new SqlCommand
            {
                Connection = _connection,
                CommandText = "[dbo].[SelectUserByPhoneNumber]"
            };

            
            if(@PhoneNumber == null)
            {
                throw new ArgumentException ("@PhoneNumber cannot be null");
            }
            
            var @PhoneNumberParameter = new SqlParameter
            {
                 Value = @PhoneNumber,
                ParameterName = "@PhoneNumber",
            };

            command.Parameters.Add(@PhoneNumberParameter);

                    


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

