using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayerWriter
{
    static class OutputHelpers
    {

        public const string IDatabaseConnectionCode = @"
            using System;
            using System.Data.SqlClient;
            using System.Data;

            namespace StructuredSight.Data
            {
                public interface IDatabaseConnection {
                    
                    IDatabaseCommand CreateCommand();
                    void Open();  
                    void Close();

                }

                public interface IDatabaseCommand {
    
                    SqlParameterCollection Parameters {get;}
                    int ExecuteNonQuery ();
                    SqlDataReader ExecuteReader ();
                    
                    String CommandText { get; set; }
                    System.Data.CommandType CommandType { get; set; }

                }
            
                public interface IDynamicExecute<T> {
    
                    T DynamicExecute (object parameters);

                }
            

            public class DatabaseCommand : IDatabaseCommand 
            {
                readonly SqlCommand _command;
                public DatabaseCommand (SqlCommand command) {
                      _command = command;   
        
                }

                public SqlParameterCollection Parameters => _command.Parameters;
            
                public CommandType CommandType
                {
                    get { return _command.CommandType; }
                    set { _command.CommandType = value; }
                }

                public string CommandText {
                    get { return _command.CommandText; }
                    set { _command.CommandText = value; }
                }

                public int ExecuteNonQuery() => _command.ExecuteNonQuery();

                public SqlDataReader ExecuteReader() => _command.ExecuteReader();
            }            

            public class DatabaseConnection : IDatabaseConnection {
                 readonly SqlConnection _connection; 
                 public DatabaseConnection(SqlConnection connection){
                    _connection = connection;
                }

                public IDatabaseCommand CreateCommand () => new DatabaseCommand(_connection.CreateCommand());


                public void Open()
                {
                    _connection.Open();
                }

                public void Close()
                {
                    _connection.Close();

                }
        
            }

        }
        
        ";

        public const string BaseClass = @"

            using System.Data.SqlClient;
            using System.Reflection;
            using dbo;
            using System.Linq; 
        
            namespace StructuredSight.Data
            {   

            public abstract class BaseAccess<T> : IDynamicExecute<T> {

            internal IDatabaseConnection Connection;

            public BaseAccess (IDatabaseConnection connection) {
                Connection = connection;
            }

            public BaseAccess (SqlConnection connection) {
            
                Connection = new DatabaseConnection(connection);

            }
    
            public T DynamicExecute(object properties)
            {
                var type = properties.GetType();

                var objectProperties = type.GetProperties().ToDictionary(x=>x.Name);

                var thisType = this.GetType();

                var executeMethod = thisType
                    .GetMethods().First(x => x.Name.Equals(""Execute""));

                var executeParameters = executeMethod.GetParameters().OrderBy(x => x.Position)
                    .Select(x =>
                    {
                        PropertyInfo tryVal;

                        return (objectProperties.TryGetValue(x.Name, out tryVal))
                            ? tryVal.GetValue(properties)
                            : null;
                    });


                return (T) 
                       executeMethod.Invoke(this, executeParameters.ToArray());
            }

        }

}


";









    }
}
