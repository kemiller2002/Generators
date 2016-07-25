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
            var allowsNull = Boolean.Parse(node.SelectSingleNode("d:Property[@Name='IsNullable']").Attributes["Value"].Value);
            var lengthString = node.SelectSingleNode("d:Property[@Name='Length']")?.Attributes["Value"].Value;

            int length;


            var typeName = node.SelectSingleNode("d:Relationship[@Name='Type']/d:Entry/d:References").Attributes["Name"].Value;

            return new Field
            {
                AllowsNull = allowsNull,
                Length =  (int.TryParse(lengthString, out length)) ? (int?)length : null ,
                Type = GetDataType(typeName)
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

            var type = (typeNode.Attributes["ExternalSource"].Value.Equals("BuiltIns"))
                ? typeNode.Attributes["Name"].Value.RemoveSquareBrackets()
                : "";

            var allowsNull = node.SelectSingleNode("d:Element/d:Property[@Name='DefaultExpressionScript']", manager) != null;

            return new Field
            {
                Name = name,
                Type = GetDataType(type),
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

        public Type GetDataType(string input) => Type.GetType($"System.{LookupDataType(input)}");

        public string LookupDataType(string input)
        {
           switch (input.ToLowerInvariant())  
            {
                case "bigint": return "Int64";

                case "binary": return "Byte[]";

                case "bit": return "Boolean";

                case "char": return "String";

                case "date": return "DateTime";

                case "datetime2": return "DateTime";

                case "datetimeoffset": return "DateTimeOffset";

                case "decimal": return "Decimal";

                case "filestream": return "Byte[]";

                case "float": return "Double";

                case "image": return "Byte[]";

                case "int": return "Int32";

                case "money": return "Decimal";

                case "nchar": return "String";

                case "ntext": return "String";

                case "numeric": return "Decimal";

                case "nvarchar": return "String";

                case "real": return "Single";

                case "rowversion": return "Byte[]";

                case "smalldatetime": return "DateTime";

                case "smallint": return "Int16";

                case "smallmoney": return "Decimal";

                case "sql_variant": return "Object";

                case "text": return "String";

                case "time": return "TimeSpan";

                case "timestamp": return "Byte[]";

                case "tinyint": return "Byte";

                case "uniqueidentifier": return "Guid";

                case "varbinary": return "Byte[]";

                case "varchar": return "String";

                case "xml": return "Xml";
            }

            throw new Exception($"Type not found: {input}");
        }

    }
}
