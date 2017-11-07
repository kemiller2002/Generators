namespace DataAccessLayerWriter

    open System;


    module StringExtensions = 


        type System.String with 
            member this.RemoveSquareBrackets() =
                if isNull this then 
                    null
                else 
                    this.Replace("[", "").Replace("]", "");

            member this.MakeWordPascal() = 
                if String.IsNullOrEmpty(this) then 
                    this
                else 
                    this.[0].ToString().ToUpper() + (Seq.skip 1 word);


            member this.MakeWordCamel () = (String.IsNullOrEmpty(word)) 
                ? word : word[0].ToString().ToLower() + word.Skip(1).Join("");

            member item.FormatStatement (converter:string -> string) =  
                item.Replace("@", "").Split([| '_', '_', '-', ' ' |], StringSplitOptions.None).
                    Select(converter).Join(",");

            member item.ToPascalCase() = item.FormatStatement(item.MakeWordPascal);

            member item.ToCamelCase () = FormatStatement(item, MakeWordCamel);


        (*

            
        member this.Join(this IEnumerable<string> items, string separator) = 
            String.Join(separator, items);

        let openJoin(this IEnumerable<char> items, string separator) = 
            String.Join(separator, items);

        *)


    
