namespace DataAccessLayerWriter


    open System.Collections.Generic;
    open System.Data;
    open System.Data.SqlClient;

    module DatabaseInteraction = 

        let CreateProcedureColumnEntry(reader:IDataReader) = 
            {
                ColumnOrdinal = reader.["column_ordinal"] :?> int;
                Name = reader.["name"].ToString(); 
                IsNullable = reader.["is_nullable"] :?> bool;
                SystemTypeId = reader.["system_type_id"] :?> int; 
                TypeName = reader.["TypeName"].ToString();
                MaxLength = -1
            };


        let GetDatabaseResult(connection:SqlConnection, schemaName:string, procedureName:string) = 

            let sql =
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
            AND dm.user_type_id IS NULL
            ORDER BY dm.name";

            use command = SqlCommand ()
            
            command.CommandText <- sql

            command.Connection = connection;
            command.Parameters.Add(new SqlParameter("@procedureName", procedureName));
            command.Parameters.Add(new SqlParameter("@schemaname", schemaName));

            use reader = command.ExecuteReader();
            
            seq {
                while  reader.Read() = true do
                    yield CreateProcedureColumnEntry(reader);
            }

