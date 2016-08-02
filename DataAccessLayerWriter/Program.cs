using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.SqlClient;
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
                @"C:\Repos\Generators\DataAccessLayerWriter\ExampleProjectForDataAccess\",
                "Data Source=localhost;Initial Catalog=ExampleDatabase;Integrated Security=SSPI"
                );
        }


        public Field GetCustomType(XmlNode node, XmlNamespaceManager manager, IEnumerable<Field> builtInTypes)
        {
            var allowsNullString =
                node.SelectSingleNode("d:Property[@Name='IsNullable']", manager)?.Attributes["Value"].Value;

            var lengthString = node.SelectSingleNode("d:Property[@Name='Length']", manager)?.Attributes["Value"].Value;

            var typeName = (node.SelectSingleNode("d:Relationship[@Name='Type']/d:Entry/d:References", manager))
                .Attributes["Name"].Value.RemoveSquareBrackets();

            var name = node.Attributes["Name"].Value;

            int lengthResult;
            bool allowsNullResult;

            return new Field
            {
                Name = name.RemoveSquareBrackets(),
                AllowsNull = (bool.TryParse(allowsNullString, out allowsNullResult)) && (bool) allowsNullResult,
                Length = (int.TryParse(lengthString, out lengthResult)) ? (int?) lengthResult : null,
                Type = builtInTypes.First(t => t.Name.Equals(typeName)).Type
            };
        }


        public Field GetCustomFieldType(XmlNode node, XmlNamespaceManager manager, IEnumerable<Field> customTypes)
        {
            var allowsNullString =
                node.SelectSingleNode("d:Property[@Name='IsNullable']", manager)?.Attributes["Value"].Value;

            var lengthString = node.SelectSingleNode("d:Property[@Name='Length']", manager)?.Attributes["Value"].Value;



            var typeName = (node.SelectSingleNode("d:Relationship[@Name='TypeSpecifier']/d:Entry/d:Element/d:Relationship/d:Entry/d:References", manager))
                .Attributes["Name"].Value.RemoveSquareBrackets();

            var name = node.Attributes["Name"].Value;

            int lengthResult;
            bool allowsNullResult;

            return new Field
            {
                Name = name.RemoveSquareBrackets(),
                AllowsNull = (bool.TryParse(allowsNullString, out allowsNullResult)) && (bool)allowsNullResult,
                Length = (int.TryParse(lengthString, out lengthResult)) ? (int?)lengthResult : null,
                Type = customTypes.First(t => t.Name.Equals(typeName)).Type
            };

        }


        public Entity GetTableType(XmlNode node, XmlNamespaceManager manager, IEnumerable<Field> customTypes)
        {
            var name = node.Attributes["Name"].Value.RemoveSquareBrackets();

            var columns = node.SelectNodes("d:Relationship/d:Entry/d:Element", manager)
                .Cast<XmlNode>().Select(n => GetCustomFieldType(n, manager, customTypes)).ToList();

            return new Entity
            {
                Name = name,
                Fields = columns,
                Type = new DataType { Name = name, IsClass = true }
            };

        }


        public bool Write(string inputFile, string outputFolder, string connectionString)
        {
            var archive = ZipFile.Open(inputFile, System.IO.Compression.ZipArchiveMode.Read);

            using (var stream = archive.Entries.First(x => x.Name.Equals("model.xml")).Open())
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open ();

                var document = new XmlDocument();
                document.Load(stream);

                var manager = new XmlNamespaceManager(document.NameTable);
                manager.AddNamespace("d", document.DocumentElement.NamespaceURI);

                var builtInDataTypes = GetBuiltInDataTypes();

                var customTypes =
                    document.SelectNodes("d:DataSchemaModel/d:Model/d:Element[@Type='SqlUserDefinedDataType']", manager)
                        .Cast<XmlNode>().Select(n => GetCustomType(n, manager, builtInDataTypes)).ToList();

                var tableTypes =
                    document.SelectNodes("d:DataSchemaModel/d:Model/d:Element[@Type='SqlTableType']", manager)
                        .Cast<XmlNode>().Select(selector: n => GetTableType(n, manager, customTypes)).ToList();


                var allTypes = builtInDataTypes.Union(customTypes)
                    .Cast<IType>().Union(tableTypes).ToDictionary(x => x.Name);

                var nodes = document.SelectNodes("d:DataSchemaModel/d:Model/d:Element", manager);

                var procedures = nodes.Cast<XmlNode>()
                    .Where(n => n.Attributes["Type"].Value.Equals("SqlProcedure"))
                    .Select(n => ParseProcedure(n, manager, allTypes, connection)).ToList();


                procedures
                    .ForEach(i => File.WriteAllText($"{outputFolder}{i.Item1}.{i.Item2}.cs", i.Item3));

                tableTypes.Select(t => DataTypeEntry.Create(t.Name, t.Fields))
                    .ToList()
                    .ForEach(i => File.WriteAllText($"{outputFolder}{i.Item1}.cs", i.Item2));

                return true;

            }

        }


        public ProcedureResult CreateProcedureResult(SqlConnection connection, string schemaName, string procedureName)
        {
            var columns = DatabaseInteraction.GetDatabaseResult(connection, schemaName, procedureName).ToArray();

            return (columns.Length == 0) ? null : new ProcedureResult
            {
                Name = procedureName,
                SchemaName = schemaName,
                Columns = columns
            };
        }                                                                                                   


        public Field CreateParameterEntry(XmlNode node, XmlNamespaceManager manager, Dictionary<string,IType> types)
        {
            var fullName = node.SelectSingleNode("d:Element", manager).Attributes["Name"].Value;
            var name = fullName.Split('.').Last().RemoveSquareBrackets();

            var typeNode =
                node.SelectSingleNode("d:Element/d:Relationship/d:Entry/d:Element/d:Relationship/d:Entry/d:References",
                    manager);

            var typeName = typeNode.Attributes["Name"].Value.RemoveSquareBrackets();

            var allowsNull = node.SelectSingleNode("d:Element/d:Property[@Name='DefaultExpressionScript']", manager) !=
                             null;

            var isOuput = node.SelectSingleNode("d:Element/d:Property[@Name='IsOutput']", manager) !=
                             null; ;

            return new Field
            {
                Name = name,
                Type = types[typeName].Type,
                AllowsNull = allowsNull,
                IsOutput = isOuput
            };
        }

        public Tuple<string, string, string> ParseProcedure(XmlNode node, XmlNamespaceManager manager, Dictionary<string,IType> types, SqlConnection connection)
        {
            var nameAttributeValue = node.Attributes["Name"].Value;
            var nameAttributeValueParts = nameAttributeValue.Split('.');

            var procedureNamespace = nameAttributeValueParts[0].RemoveSquareBrackets();
            var procedureName = nameAttributeValueParts[1].RemoveSquareBrackets();

            var procedureResult = CreateProcedureResult(connection, procedureNamespace, procedureName);

            var parameters = node.SelectNodes("d:Relationship[@Name='Parameters']/d:Entry", manager)
                .Cast<XmlNode>().Select(n => CreateParameterEntry(n, manager, types));

            return new Tuple<string, string, string>(procedureNamespace, procedureName,
                ProcedureEntry.Create(procedureNamespace, procedureName, parameters, procedureResult, types));
        }

        public IEnumerable<Field> GetBuiltInDataTypes()
        {

            yield return new Field()
            {
                Name = "bigint",
                Type = Type.GetType("System.Int64").ToDataType()
            };

            yield return new Field()
            {
                Name = "binary",
                Type = typeof(System.Byte[]).ToDataType()
            };

            yield return new Field()
            {
                Name = "bit",
                Type = typeof(System.Boolean).ToDataType()
            };

            yield return new Field()
            {
                Name = "char",
                Type = typeof(System.String).ToDataType()
            };

            yield return new Field()
            {
                Name = "date",
                Type = typeof(System.DateTime).ToDataType()
            };

            yield return new Field()
            {
                Name = "datetime2",
                Type = typeof(System.DateTime).ToDataType()
            };

            yield return new Field()
            {
                Name = "datetimeoffset",
                Type = typeof(System.DateTimeOffset).ToDataType()
            };

            yield return new Field()
            {
                Name = "decimal",
                Type = typeof(System.Decimal).ToDataType()
            };

            yield return new Field()
            {
                Name = "filestream",
                Type = typeof(System.Byte[]).ToDataType()
            };

            yield return new Field()
            {
                Name = "float",
                Type = typeof(System.Double).ToDataType()
            };

            yield return new Field()
            {
                Name = "image",
                Type = typeof(System.Byte[]).ToDataType()
            };

            yield return new Field()
            {
                Name = "int",
                Type = typeof(System.Int32).ToDataType()
            };

            yield return new Field()
            {
                Name = "money",
                Type = typeof(System.Decimal).ToDataType()
            };

            yield return new Field()
            {
                Name = "nchar",
                Type = typeof(System.String).ToDataType()
            };

            yield return new Field()
            {
                Name = "ntext",
                Type = typeof(System.String).ToDataType()
            };

            yield return new Field()
            {
                Name = "numeric",
                Type = typeof(System.Decimal).ToDataType()
            };

            yield return new Field()
            {
                Name = "nvarchar",
                Type = typeof(System.String).ToDataType()
            };

            yield return new Field()
            {
                Name = "real",
                Type = typeof(System.Single).ToDataType()
            };

            yield return new Field()
            {
                Name = "rowversion",
                Type = typeof(System.Byte[]).ToDataType()
            };

            yield return new Field()
            {
                Name = "smalldatetime",
                Type = typeof(System.DateTime).ToDataType()
            };

            yield return new Field()
            {
                Name = "smallint",
                Type = typeof(System.Int16).ToDataType()
            };

            yield return new Field()
            {
                Name = "smallmoney",
                Type = typeof(System.Decimal).ToDataType()
            };

            yield return new Field()
            {
                Name = "sql_variant",
                Type = typeof(System.Object).ToDataType()
            };

            yield return new Field()
            {
                Name = "text",
                Type = typeof(System.String).ToDataType()
            };

            yield return new Field()
            {
                Name = "time",
                Type = typeof(System.TimeSpan).ToDataType()
            };

            yield return new Field()
            {
                Name = "timestamp",
                Type = typeof(System.Byte[]).ToDataType()
            };

            yield return new Field()
            {
                Name = "tinyint",
                Type = typeof(System.Byte).ToDataType()
            };

            yield return new Field()
            {
                Name = "uniqueidentifier",
                Type = typeof(System.Guid).ToDataType()
            };

            yield return new Field()
            {
                Name = "varbinary",
                Type = typeof(System.Byte[]).ToDataType()
            };

            yield return new Field()
            {
                Name = "varchar",
                Type = typeof(System.String).ToDataType()
            };

            yield return new Field()
            {
                Name = "xml",
                Type = typeof(XmlDocument).ToDataType()
            };
        }

    }

}

