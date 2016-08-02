using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessLayerWriter
{
    public static class DatabaseInteraction
    {

        public static IEnumerable<ProcedureColumnEntry> GetDatabaseResult(SqlConnection connection, string schemaName, string procedureName)
        {

            var sql =
                @"SELECT column_ordinal, dm.name, dm.is_nullable, dm.system_type_id, t.name as TypeName, dm.max_length
            FROM sys.dm_exec_describe_first_result_set_for_object
            (
              (
            	SELECT TOP 1 p.object_id FROM sys.procedures p

                JOIN sys.schemas s ON p.schema_id = s.schema_id

                WHERE p.name = @procedureName AND s.name = @schemaName
              ), 
              NULL
            ) dm
            JOIN sys.types t

                ON t.system_type_id = dm.system_type_id
                AND t.system_type_id = t.user_type_id
                -- did this to remove all the duplicates from custom types.
            WHERE t.name<> 'sysname' 
            ORDER BY dm.name";

            using (var command = new SqlCommand {CommandText = sql})
            {
                command.Connection = connection;
                command.Parameters.Add(new SqlParameter("@procedureName", procedureName));
                command.Parameters.Add(new SqlParameter("@schemaname", schemaName));

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    yield return CreateProcedureColumnEntry(reader);
                }

                reader.Close();
            }

        }

        public static ProcedureColumnEntry CreateProcedureColumnEntry(IDataReader reader)
        {
            return new ProcedureColumnEntry
            {
                ColumnOrdinal = (int)reader["column_ordinal"], 
                Name = reader["name"].ToString(), 
                IsNullable = (bool)reader["is_nullable"],
                SystemTypeId = (int) reader["system_type_id"], 
            };
        }

    }
}