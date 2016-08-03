

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Net.Configuration;



                namespace dbo
                {
                    public class SelectUsersResultEntry
                    {
                        public SelectUsersResultEntry (IDataReader reader)
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
                        public class SelectUsersResult 
                        {
                             public dbo.SelectUsers Recordset {get;set;}
                    
                            
                        }

                    }
    
    
                

namespace dbo
{

    public class SelectUsers
    {

        readonly SqlConnection _connection;

        public SelectUsers(SqlConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<dbo.SelectUsersResult> Execute(ref Int32? @Id,  String @FirstName)
        {
            var command = new SqlCommand
            {
                Connection = _connection,
                CommandText = "[dbo].[SelectUsers]"
            };

            
            if (@Id.HasValue)
            {
                var parameter = new SqlParameter
                {
                    Value = @Id,
                    ParameterName = "@Id",
                };

                command.Parameters.Add(parameter);
                
            }

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

                    


            
            var reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                yield return new dbo.SelectUsersResult(reader);
            }
            
        }


    }

}

