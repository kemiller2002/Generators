

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using StructuredSight.Data;


                namespace dbo
                {
                    
                    public class GetUsersByPhoneNumberListRecord
                    {
                        public GetUsersByPhoneNumberListRecord (IDataRecord reader)
                        {
                            EmailAddress = (reader["EmailAddress"] is DBNull) ? default(String) : (String)reader["EmailAddress"] ;
FirstName = (reader["FirstName"] is DBNull) ? default(String) : (String)reader["FirstName"] ;
Id = (reader["Id"] is DBNull) ? default(Int32) : (Int32)reader["Id"] ;
LastName = (reader["LastName"] is DBNull) ? default(String) : (String)reader["LastName"] ;
PhoneNumber = (reader["PhoneNumber"] is DBNull) ? default(String) : (String)reader["PhoneNumber"] ;
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
                             public IEnumerable<dbo.GetUsersByPhoneNumberListRecord> Recordset {get;set;}
                    
                            
                        }

                    }
                

namespace dbo
{

    public class GetUsersByPhoneNumberListExecutionObject 
    {
        IEnumerable<dbo.PhoneNumbers> PhoneNumbers {get;set;}    
    }

    public class GetUsersByPhoneNumberList : StructuredSight.Data.BaseAccess<GetUsersByPhoneNumberListResult>
    {

        public GetUsersByPhoneNumberList(SqlConnection connection) : base(connection)
        {
        }

        public dbo.GetUsersByPhoneNumberListResult Execute(IEnumerable<dbo.PhoneNumbers> phoneNumbers)
        {
            var command = Connection.CreateCommand();
            command.CommandText = "[dbo].[GetUsersByPhoneNumberList]";
            command.CommandType = CommandType.StoredProcedure;


            

             if(phoneNumbers == null)
                    {
                        throw new ArgumentException("@PhoneNumbers cannot be null");
                    }
                
            if(phoneNumbers != null)
            {
                var @PhoneNumbersParameter = new SqlParameter
                {
                     Value = phoneNumbers,
                    ParameterName = "@PhoneNumbers"
                    
                    , SqlDbType =  SqlDbType.BigInt 
                };
                
                command.Parameters.Add(@PhoneNumbersParameter);
            }
            
            


            
            var reader = command.ExecuteReader();
            

            return new dbo.GetUsersByPhoneNumberListResult
                {
                    
                    Recordset = (from IDataRecord r in reader select new dbo.GetUsersByPhoneNumberListRecord  (r) )
                };
            
        }

        public dbo.GetUsersByPhoneNumberListResult ExecuteWithExecutionObject(GetUsersByPhoneNumberListExecutionObject input)
        {
            
            return Execute(phoneNumbers : input.PhoneNumbers);
        
        }

    }

}

