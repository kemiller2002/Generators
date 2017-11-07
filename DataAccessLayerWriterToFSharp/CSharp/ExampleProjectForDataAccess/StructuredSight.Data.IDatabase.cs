
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
        
        