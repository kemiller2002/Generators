

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;



                namespace dbo
                {
                    
                    /*
                        The following columns have duplicate entries.  
                        You'll need to either alias a column or remove the duplicate name in the query.
                        
                        

                    */
            
                    public class SelectRecordsetWithNoColumnNameRecord
                    {
                        public SelectRecordsetWithNoColumnNameRecord (IDataRecord reader)
                        {
                             = (Int32)reader[""] ;
 = (Int32)reader[""] ;
                        }

                        public Int32 {get;set;}
public Int32 {get;set;}

                    }
                }
            


                    namespace dbo
                    {
                        public class SelectRecordsetWithNoColumnNameResult
                        {
                             public IEnumerable<dbo.SelectRecordsetWithNoColumnNameRecord> Recordset {get;set;}
                    
                            
                        }

                    }
                

namespace dbo
{

    public class SelectRecordsetWithNoColumnName
    {

        readonly SqlConnection _connection;

        public SelectRecordsetWithNoColumnName(SqlConnection connection)
        {
            _connection = connection;
        }

        public dbo.SelectRecordsetWithNoColumnNameResult Execute(Int32? @param1, Int32 @param2)
        {
            var command = new SqlCommand
            {
                Connection = _connection,
                CommandText = "[dbo].[SelectRecordsetWithNoColumnName]",
                CommandType = CommandType.StoredProcedure
            };


            
            if (@param1.HasValue)
            {
                var parameter = new SqlParameter
                {
                    Value = @param1,
                    ParameterName = "@param1"
                    
                };

                command.Parameters.Add(parameter);
                
            }


            
            if(@param2 != null)
            {
                var @param2Parameter = new SqlParameter
                {
                     Value = @param2,
                    ParameterName = "@param2"
                    
                };
                
                command.Parameters.Add(@param2Parameter);
            }
            
            


            
            var reader = command.ExecuteReader();
            

            return new dbo.SelectRecordsetWithNoColumnNameResult
                {
                    
                    Recordset = (from IDataRecord r in reader select new dbo.SelectRecordsetWithNoColumnNameRecord  (r) )
                };
            
        }

    }

}

