// Learn more about F# at http://fsharp.org
namespace DataAccessLayerWriter

open System
open System.Xml

open StringExtensions

module Program = 

    let GetAttributeValue (node:XmlNode)(manager:XmlNamespaceManager)(path:string)(attributeName:string) = 
        let result = node.SelectSingleNode(path, manager)

        if isNull result then 
            None 
        else 
            Some (result.Attributes.[attributeName].Value) 

    let GetCustomType (node:XmlNode) ( manager:XmlNamespaceManager)(builtInTypes : Field seq)  =
        let getAttributeValue = GetAttributeValue node manager

        let allowsNullString = getAttributeValue "d:Property[@Name='IsNullable']" "Value"

        let lengthString = getAttributeValue "d:Property[@Name='Length']" "Value"

        let typeName = 
           getAttributeValue "d:Relationship[@Name='Type']/d:Entry/d:References" "Name"

        let name = node.Attributes.["Name"].Value;

        {
            Name = name.RemoveSquareBrackets()

            AllowsNull = (bool.TryParse(allowsNullString, out allowsNullResult)) && (bool) allowsNullResult,
            Length = (int.TryParse(lengthString, out lengthResult)) ? (int?) lengthResult : null;
            Type = builtInTypes.First(t => t.Name.Equals(typeName)).Type
        }






    let instructions = @"
The program requires 3 parameters.
    1. Location of the compiled dacpac.
    2. The output folder for the desired data access classes
            The output folder location should not have trailing a trailing backslash (\) at the end.

            NOTE: These will not be automatically added to the visual studio project.
    3.  The connection string to access the database where the dacpac has been deployed."


    [<EntryPoint>]
    let main argv =
        //let writer = Writer();

        if (argv.Length < 3) then
            Console.WriteLine(instructions) |> ignore
            1 
        else 
            //writer.Write argv.[0] argv.[1] argv.[2]
            0