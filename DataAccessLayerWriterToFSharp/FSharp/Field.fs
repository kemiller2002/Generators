namespace DataAccessLayerWriter

    open System;

    type IType = {
        Name:string;
        DataType:Type
    }

    type Field = {
         Name:string; 
         Type:DataType; 
         AllowsNull:bool;
         Length:int option;
         IsOutput:bool
    }
