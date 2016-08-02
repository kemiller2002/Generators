

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Net.Configuration;



                namespace dbo
                {
                    public class SelectUsersResult
                    {
                        public SelectUsersResult (IDataReader reader)
                        {
                            SelectUsers
SelectUsers
SelectUsers
SelectUsers
SelectUsers
                        }

                        public  EmailAddress{get;set;}
public  FirstName{get;set;}
public  Id{get;set;}
public  LastName{get;set;}
public  PhoneNumber{get;set;}

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

        public IEnumerable<dbo.SelectUsersResult> Execute(Int32? @Id, String @FirstName)
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

