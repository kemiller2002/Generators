using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Xml;

namespace DataAccessLayerWriter
{

    class Writer
    {
        static void Main(string[] args)
        {
            var writer = new Writer();
            writer.Write(
                @"C:\Repos\Generators\ExampleDatabase\bin\Debug\ExampleDatabase.dacpac",
                "C:\\temp\\"
                );
        }


        public Field GetCustomType(XmlNode node, XmlNamespaceManager manager)
        {

            /*
             <Element Type="SqlUserDefinedDataType" Name="[dbo].[PhoneNumber]">
			<Property Name="IsNullable" Value="False" />
			<Property Name="Length" Value="10" />
			<Relationship Name="Schema">
				<Entry>
					<References ExternalSource="BuiltIns" Name="[dbo]" />
				</Entry>
			</Relationship>
			<Relationship Name="Type">
				<Entry>
					<References ExternalSource="BuiltIns" Name="[varchar]" />
				</Entry>
			</Relationship>
		</Element>
             
             */
            var allowsNullString =
                node.SelectSingleNode("d:Property[@Name='IsNullable']", manager)?.Attributes["Value"].Value;

            var lengthString = node.SelectSingleNode("d:Property[@Name='Length']", manager)?.Attributes["Value"].Value;



            var typeName = node.SelectSingleNode("d:Relationship[@Name='Type']/d:Entry/d:References", manager)
                .Attributes["Name"].Value.RemoveSquareBrackets();

            var name = node.Attributes["Name"].Value;

            int lengthResult;
            bool allowsNullResult;

            return new Field
            {
                Name = name,
                AllowsNull = (bool.TryParse(allowsNullString,out allowsNullResult)) && (bool) allowsNullResult,
                Length =  (int.TryParse(lengthString, out lengthResult)) ? (int?)lengthResult : null ,
                Type = GetDataType(typeName, node, manager)
            };
        }



        public bool Write(string inputFile, string outputFolder)
        {
            var archive = ZipFile.Open(inputFile ,System.IO.Compression.ZipArchiveMode.Read);

            using (var stream = archive.Entries.First(x => x.Name.Equals("model.xml")).Open())
            {
                var document = new XmlDocument();
                document.Load(stream);

                var manager = new XmlNamespaceManager(document.NameTable);
                manager.AddNamespace("d", document.DocumentElement.NamespaceURI);

                var customTypeNodes = document.SelectNodes("d:DataSchemaModel/d:Model/d:Element[@Type='SqlUserDefinedDataType']", manager)
                    .Cast<XmlNode>().Select(n => GetCustomType(n, manager)).ToList();


                var nodes = document.SelectNodes("d:DataSchemaModel/d:Model/d:Element", manager);

                var code = nodes.Cast<XmlNode>()
                     .Where(n => n.Attributes["Type"].Value.Equals("SqlProcedure"))
                     .Select(n => ParseProcedure(n, manager));


                code.ToList().ForEach(i=>File.WriteAllText($"{outputFolder}{i.Item1}", i.Item2));

                return true;

            }

        }

        public String GetCustomTypeName(string name, XmlNode node, XmlNamespaceManager manager)
        {
            throw new NotImplementedException();
        }

        public Field CreateParameterEntry(XmlNode node, XmlNamespaceManager manager)
        {
            var fullName = node.SelectSingleNode("d:Element", manager).Attributes["Name"].Value;
            var name = fullName.Split('.').Last().RemoveSquareBrackets();

            var typeNode = node.SelectSingleNode("d:Element/d:Relationship/d:Entry/d:Element/d:Relationship/d:Entry/d:References", manager);

            var type = typeNode.Attributes["Name"].Value.RemoveSquareBrackets();
                
                //(typeNode.Attributes["ExternalSource"]?.Value == "BuiltIns")
                //? typeNode.Attributes["Name"].Value.RemoveSquareBrackets()
                //: "";

            var allowsNull = node.SelectSingleNode("d:Element/d:Property[@Name='DefaultExpressionScript']", manager) != null;

            return new Field
            {
                Name = name,
                Type = GetDataType(type, node, manager),
                AllowsNull = allowsNull
            };
        }
        
        public Tuple<string,string> ParseProcedure(XmlNode node, XmlNamespaceManager manager)
        {
            var nameAttributeValue = node.Attributes["Name"].Value;
            var nameAttributeValueParts = nameAttributeValue.Split('.');

            var procedureNamespace = nameAttributeValueParts[0].RemoveSquareBrackets();
            var procedureName = nameAttributeValueParts[1].RemoveSquareBrackets();

            var parameters = node.SelectNodes("d:Relationship[@Name='Parameters']/d:Entry", manager)
                .Cast<XmlNode>().Select(n => CreateParameterEntry(n, manager));

            return new Tuple<string, string>($"{procedureNamespace}.{procedureName}.cs", 
                    ProcedureEntry.Create(procedureNamespace, procedureName, parameters));
        }


        public Type GetDataType(string input, IEnumerable<Field> customTypes) => Type.GetType($"System.{LookupDataType(input, customTypes)}");

        public IEnumerable<Field> LookupDataType(string input, IEnumerable<Field> customTypes)
        {
            if (maxRecursionDepth == 0)
            {
                throw new Exception($"Type not found: {input}");
            }

            switch (input.ToLowerInvariant())
            {
                case "bigint":
                   yield return new Field()
                    {
                        Name = "bigint",
                        Type = Type.GetType("System.Int64")
                    };

                case "binary":
                    return Type.GetType("System.Byte[]");

                case "bit":
                    return Type.GetType("System.Boolean");

                case "char":
                    return Type.GetType("System.String");

                case "date":
                    return Type.GetType("System.DateTime");

                case "datetime2":
                    return Type.GetType("System.DateTime");

                case "datetimeoffset":
                    return Type.GetType("System.DateTimeOffset");

                case "decimal":
                    return Type.GetType("System.Decimal");

                case "filestream":
                    return Type.GetType("System.Byte[]");

                case "float":
                    return Type.GetType("System.Double");

                case "image":
                    return Type.GetType("System.Byte[]");

                case "int":
                    return Type.GetType("System.Int32");

                case "money":
                    return Type.GetType("System.Decimal");

                case "nchar":
                    return Type.GetType("System.String");

                case "ntext":
                    return Type.GetType("System.String");

                case "numeric":
                    return Type.GetType("System.Decimal");

                case "nvarchar":
                    return Type.GetType("System.String");

                case "real":
                    return Type.GetType("System.Single");

                case "rowversion":
                    return Type.GetType("System.Byte[]");

                case "smalldatetime":
                    return Type.GetType("System.DateTime");

                case "smallint":
                    return Type.GetType("System.Int16");

                case "smallmoney":
                    return Type.GetType("System.Decimal");

                case "sql_variant":
                    return Type.GetType("System.Object");

                case "text":
                    return Type.GetType("System.String");

                case "time":
                    return Type.GetType("System.TimeSpan");

                case "timestamp":
                    return Type.GetType("System.Byte[]");

                case "tinyint":
                    return Type.GetType("System.Byte");

                case "uniqueidentifier":
                    return Type.GetType("System.Guid");

                case "varbinary":
                    return Type.GetType("System.Byte[]");

                case "varchar":
                    return Type.GetType("System.String");

                case "xml":
                    return Type.GetType("System.Xml");
            }

            return LookupDataType(input, node, manager, --maxRecursionDepth);
        }

    }
}
