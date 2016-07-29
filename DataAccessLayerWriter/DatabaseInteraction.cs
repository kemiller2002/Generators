namespace DataAccessLayerWriter
{
    public class DatabaseInteraction
    {
        "SELECT column_ordinal, dm.name, dm.is_nullable, dm.system_type_id, t.name as TypeName, dm.max_length
            FROM sys.dm_exec_describe_first_result_set_for_object
            (
              (
            	SELECT TOP 1 p.object_id FROM sys.procedures p

                JOIN sys.schemas s ON p.schema_id = s.schema_id

                WHERE p.name = 'SelectUserByPhoneNumber' AND s.name = 'dbo'
              ), 
              NULL
            ) dm
            JOIN sys.types t

                ON t.system_type_id = dm.system_type_id
                AND t.system_type_id = t.user_type_id
                -- did this to remove all the duplicates from custom types.
            WHERE t.name<> 'sysname' 
            ORDER BY dm.name"










    }
}