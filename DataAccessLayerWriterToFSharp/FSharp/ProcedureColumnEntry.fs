namespace DataAccessLayerWriter

    type ProcedureColumnEntry = 
        {
            ColumnOrdinal:int; 
            Name:string; 
            IsNullable:bool; 
            SystemTypeId:int; 
            TypeName:string; 
            MaxLength:int;  
        }


    type ProcedureResult = {
         SchemaName:string; 
         Name:string; 
         Columns:ProcedureColumnEntry seq
    }

