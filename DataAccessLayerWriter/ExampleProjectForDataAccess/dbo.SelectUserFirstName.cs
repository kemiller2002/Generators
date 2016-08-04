

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Net.Configuration;
using System.Linq;





                    namespace dbo
                    {
                        public class SelectUserFirstNameResult
                        {
                            public int RecordsAffected {get;set;}
                    
                            public String @FirstName {get;set;}
                        }

                    }
                

namespace dbo
{

    public class SelectUserFirstName
    {

        readonly SqlConnection _connection;

        public SelectUserFirstName(SqlConnection connection)
        {
            _connection = connection;
        }

        public dbo.SelectUserFirstNameResult Execute(String @PhoneNumber, String @FirstName)
        {
            var command = new SqlCommand
            {
                Connection = _connection,
                CommandText = "[dbo].[SelectUserFirstName]",
                CommandType = CommandType.StoredProcedure
            };


            

             if(@PhoneNumber == null)
                    {
                        throw new ArgumentException("@PhoneNumber cannot be null");
                    }
                
            
            var @PhoneNumberParameter = new SqlParameter
            {
                 Value = @PhoneNumber,
                ParameterName = "@PhoneNumber"
                
            };

            command.Parameters.Add(@PhoneNumberParameter);

                    


             if(@FirstName == null)
                    {
                        throw new ArgumentException("@FirstName cannot be null");
                    }
                
            
            var @FirstNameParameter = new SqlParameter
            {
                 Value = @FirstName,
                ParameterName = "@FirstName"
                , Direction = ParameterDirection.InputOutput 
            };

            command.Parameters.Add(@FirstNameParameter);

                    


            var result = command.ExecuteNonQuery();
                    
                return new dbo.SelectUserFirstNameResult
                {
                    @FirstName = (String)command.Parameters["@FirstName"].Value,
RecordsAffected = result
                };
                
        }


    }

}

