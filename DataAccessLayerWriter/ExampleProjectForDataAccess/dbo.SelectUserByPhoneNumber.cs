

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Net.Configuration;
using System.Linq;



                namespace dbo
                {
                    public class SelectUserByPhoneNumberRecord
                    {
                        public SelectUserByPhoneNumberRecord (IDataRecord reader)
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
                             public IEnumerable<dbo.SelectUserByPhoneNumberRecord> Recordset {get;set;}
                    
                            
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

        public dbo.SelectUserByPhoneNumberResult Execute(String @PhoneNumber)
        {
            var command = new SqlCommand
            {
                Connection = _connection,
                CommandText = "[dbo].[SelectUserByPhoneNumber]",
                CommandType = CommandType.StoredProcedure
            };


            

             if(@PhoneNumber == null)
                    {
                        throw new ArgumentException("@PhoneNumber cannot be null");
                    }
                
            if(@PhoneNumber != null)
            {
                var @PhoneNumberParameter = new SqlParameter
                {
                     Value = @PhoneNumber,
                    ParameterName = "@PhoneNumber"
                    
                    , SqlDbType =  SqlDbType.VarChar 
                };
                
                command.Parameters.Add(@PhoneNumberParameter);
            }
            
            


            
            var reader = command.ExecuteReader();
            

            return new dbo.SelectUserByPhoneNumberResult
                {
                    
                    Recordset = (from IDataRecord r in reader select new dbo.SelectUserByPhoneNumberRecord  (r) )
                };
            
        }

    }

}

