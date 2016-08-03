

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Net.Configuration;



                namespace dbo
                {
                    public class SelectUserByPhoneNumberResultEntry
                    {
                        public SelectUserByPhoneNumberResultEntry (IDataReader reader)
                        {
                            EmailAddress = (String)reader["EmailAddress"] ;
FirstName = (String)reader["FirstName"] ;
Id = (Int32)reader["Id"] ;
LastName = (String)reader["LastName"] ;
                        }

                        public String EmailAddress{get;set;}
public String FirstName{get;set;}
public Int32 Id{get;set;}
public String LastName{get;set;}

                    }
                }
            


                    namespace dbo
                    {
                        public class SelectUserByPhoneNumberResult 
                        {
                             public dbo.SelectUserByPhoneNumber Recordset {get;set;}
                    
                            
                        }

                    }
    
    
                

namespace dbo
{

    public class SelectUserByPhoneNumber
    {

        readonly SqlConnection _connection;

        public SelectUserByPhoneNumber(SqlConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<dbo.SelectUserByPhoneNumberResult> Execute( String @PhoneNumber)
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
                yield return new dbo.SelectUserByPhoneNumberResult(reader);
            }
            
        }


    }

}

