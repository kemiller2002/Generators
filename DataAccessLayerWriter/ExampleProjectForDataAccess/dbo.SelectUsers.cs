

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using StructuredSight.Data;


                namespace dbo
                {
                    
                    public class SelectUsersRecord
                    {
                        public SelectUsersRecord (IDataRecord reader)
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
                        public class SelectUsersResult
                        {
                             public IEnumerable<dbo.SelectUsersRecord> Recordset {get;set;}
                    
                            
                        }

                    }
                

namespace dbo
{

    public class SelectUsersExecutionObject 
    {
        Int32? Id {get;set;}
String FirstName {get;set;}    
    }

    public class SelectUsers : StructuredSight.Data.BaseAccess<SelectUsersResult>
    {

        public SelectUsers(SqlConnection connection) : base(connection)
        {
        }

        public dbo.SelectUsersResult Execute(Int32? id, String firstName)
        {
            var command = Connection.CreateCommand();
            command.CommandText = "[dbo].[SelectUsers]";
            command.CommandType = CommandType.StoredProcedure;


            
            if (id.HasValue)
            {
                var parameter = new SqlParameter
                {
                    Value = id,
                    ParameterName = "@Id"
                    
                    , SqlDbType =  SqlDbType.Int 
                };

                command.Parameters.Add(parameter);
                
            }


             if(firstName == null)
                    {
                        throw new ArgumentException("@FirstName cannot be null");
                    }
                
            if(firstName != null)
            {
                var @FirstNameParameter = new SqlParameter
                {
                     Value = firstName,
                    ParameterName = "@FirstName"
                    
                    , SqlDbType =  SqlDbType.VarChar 
                };
                
                command.Parameters.Add(@FirstNameParameter);
            }
            
            


            
            var reader = command.ExecuteReader();
            

            return new dbo.SelectUsersResult
                {
                    
                    Recordset = (from IDataRecord r in reader select new dbo.SelectUsersRecord  (r) )
                };
            
        }

        public dbo.SelectUsersResult ExecuteWithExecutionObject(SelectUsersExecutionObject input)
        {
            
            return Execute(id : input.Id, firstName : input.FirstName);
        
        }

    }

}

