

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using StructuredSight.Data;




                    namespace dbo
                    {
                        public class SelectUserFirstNameResult
                        {
                            public int RecordsAffected {get;set;}
                    
                            public String FirstName {get;set;}
                        }

                    }
                

namespace dbo
{

    public class SelectUserFirstNameExecutionObject 
    {
        String PhoneNumber {get;set;}
String FirstName {get;set;}    
    }

    public class SelectUserFirstName : StructuredSight.Data.BaseAccess<SelectUserFirstNameResult>
    {

        public SelectUserFirstName(SqlConnection connection) : base(connection)
        {
        }

        public dbo.SelectUserFirstNameResult Execute(String phoneNumber, String firstName)
        {
            var command = Connection.CreateCommand();
            command.CommandText = "[dbo].[SelectUserFirstName]";
            command.CommandType = CommandType.StoredProcedure;


            

             if(phoneNumber == null)
                    {
                        throw new ArgumentException("@PhoneNumber cannot be null");
                    }
                
            if(phoneNumber != null)
            {
                var @PhoneNumberParameter = new SqlParameter
                {
                     Value = phoneNumber,
                    ParameterName = "@PhoneNumber"
                    
                    , SqlDbType =  SqlDbType.VarChar 
                };
                
                command.Parameters.Add(@PhoneNumberParameter);
            }
            
            


            
            if(firstName != null)
            {
                var @FirstNameParameter = new SqlParameter
                {
                     Value = firstName,
                    ParameterName = "@FirstName"
                    , Direction = ParameterDirection.InputOutput 
                    , SqlDbType =  SqlDbType.VarChar 
                };
                
                command.Parameters.Add(@FirstNameParameter);
            }
            else {
                    var parameter = new SqlParameter
                {
                    ParameterName = "@FirstName"
                    , Direction = ParameterDirection.InputOutput 
                    , Size = 10
                    , SqlDbType =  SqlDbType.VarChar 
                };

                command.Parameters.Add(parameter);
                

            }
            


            var result = command.ExecuteNonQuery();
                    
                return new dbo.SelectUserFirstNameResult
                {
                    FirstName = (String)command.Parameters["@FirstName"].Value,
RecordsAffected = result
                };
                
        }

        public dbo.SelectUserFirstNameResult ExecuteWithExecutionObject(SelectUserFirstNameExecutionObject input)
        {
            
            return Execute(phoneNumber : input.PhoneNumber, firstName : input.FirstName);
        
        }

    }

}

