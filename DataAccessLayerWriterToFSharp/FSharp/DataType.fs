namespace DataAccessLayerWriter

open System.Data;

type DataType = {
        Name:string; 

        IsClass:bool; 

        SqlDefaultSize:int; 

        SqlDbType:SqlDbType; 
    }