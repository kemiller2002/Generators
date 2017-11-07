

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using StructuredSight.Data;


                namespace dbo
                {
                    
                    public class SelectUserByPhoneNumberRecord
                    {
                        public SelectUserByPhoneNumberRecord (IDataRecord reader)
                        {
                            EmailAddress = (reader["EmailAddress"] is DBNull) ? default(String) : (String)reader["EmailAddress"] ;
FirstName = (reader["FirstName"] is DBNull) ? default(String) : (String)reader["FirstName"] ;
Id = (reader["Id"] is DBNull) ? default(Int32) : (Int32)reader["Id"] ;
LastName = (reader["LastName"] is DBNull) ? default(String) : (String)reader["LastName"] ;
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

    public class SelectUserByPhoneNumberExecutionObject 
    {
        String PhoneNumber {get;set;}    
    }

    public class SelectUserByPhoneNumber : StructuredSight.Data.BaseAccess<SelectUserByPhoneNumberResult>
    {

        public SelectUserByPhoneNumber(SqlConnection connection) : base(connection)
        {
        }

        public dbo.SelectUserByPhoneNumberResult Execute(String phoneNumber)
        {
            var command = Connection.CreateCommand();
            command.CommandText = "[dbo].[SelectUserByPhoneNumber]";
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
            
            


            
            var reader = command.ExecuteReader();
            

            return new dbo.SelectUserByPhoneNumberResult
                {
                    
                    Recordset = (from IDataRecord r in reader select new dbo.SelectUserByPhoneNumberRecord  (r) )
                };
            
        }

        public dbo.SelectUserByPhoneNumberResult ExecuteWithExecutionObject(SelectUserByPhoneNumberExecutionObject input)
        {
            
            return Execute(phoneNumber : input.PhoneNumber);
        
        }

    }

}

