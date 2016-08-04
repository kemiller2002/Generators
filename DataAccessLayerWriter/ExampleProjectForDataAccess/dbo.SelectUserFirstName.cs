

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
                            int RecordsAffected {get;set;}
                    
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
                CommandText = "[dbo].[SelectUserFirstName]"
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

                    


            var result = command.Execute()
                    
                return new dboSelectUserFirstNameResult
                {
                    @FirstName = command.@FirstName.Value,
RecordsAffected = result
                };
                
        }


    }

}

