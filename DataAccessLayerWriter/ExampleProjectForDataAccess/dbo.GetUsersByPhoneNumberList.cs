

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Net.Configuration;



                namespace dbo
                {
                    public class GetUsersByPhoneNumberListResultEntry
                    {
                        public GetUsersByPhoneNumberListResultEntry (IDataReader reader)
                        {
                            EmailAddress = (String)reader["EmailAddress"] ;
FirstName = (String)reader["FirstName"] ;
Id = (Int32)reader["Id"] ;
LastName = (String)reader["LastName"] ;
PhoneNumber = (String)reader["PhoneNumber"] ;
                        }

                        public String EmailAddress{get;set;}
public String FirstName{get;set;}
public Int32 Id{get;set;}
public String LastName{get;set;}
public String PhoneNumber{get;set;}

                    }
                }
            


                    namespace dbo
                    {
                        public class GetUsersByPhoneNumberListResult 
                        {
                             public dbo.GetUsersByPhoneNumberList Recordset {get;set;}
                    
                            
                        }

                    }
    
    
                

namespace dbo
{

    public class GetUsersByPhoneNumberList
    {

        readonly SqlConnection _connection;

        public GetUsersByPhoneNumberList(SqlConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<dbo.GetUsersByPhoneNumberListResult> Execute( dbo.PhoneNumbers @PhoneNumbers)
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
                yield return new dbo.GetUsersByPhoneNumberListResult(reader);
            }
            
        }


    }

}

