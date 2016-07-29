

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Net.Configuration;


namespace dbo
{

    public class GetUsersByPhoneNumberList
    {

        readonly SqlConnection _connection;

        public GetUsersByPhoneNumberList(SqlConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<dynamic> Execute(dbo.PhoneNumbers @PhoneNumbers)
        {
            var command = new SqlCommand
            {
                Connection = _connection,
                CommandText = "[dbo].[GetUsersByPhoneNumberList]"
            };

            
            if(@PhoneNumbers == null)
            {
                throw new ArgumentException ("@PhoneNumbers cannot be null");
            }
            
            var @PhoneNumbersParameter = new SqlParameter
            {
                 Value = @PhoneNumbers,
                ParameterName = "@PhoneNumbers",
            };

            command.Parameters.Add(@PhoneNumbersParameter);

                    


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

