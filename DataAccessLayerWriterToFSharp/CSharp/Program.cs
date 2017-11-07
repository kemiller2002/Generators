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

            if (args.Length < 3)
                {
                    Console.WriteLine(@"
The program requires 3 parameters.
    1. Location of the compiled dacpac.
    2. The output folder for the desired data access classes
            The output folder location should not have trailing a trailing backslash (\) at the end.

            NOTE: These will not be automatically added to the visual studio project.
    3.  The connection string to access the database where the dacpac has been deployed.
             

                    ");
                    return;
                }
                        writer.Write(
                args[0], 
                args[1],
                args[2]
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



            var typeName = (
                    node
                    .SelectSingleNode
                        ("d:Relationship[@Name='TypeSpecifier']/d:Entry/d:Element/d:Relationship/d:Entry/d:References", manager))
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


            //putting ienumerable around name, because the table type is a collection of this object
            //when other code uses this entity, it won't have to know to explicity convert it to an 
            //enumeration. 
            return new Entity
            {
                Name = name,
                Fields = columns,
                Type = new DataType { Name = $"IEnumerable<{name}>", IsClass = true }
            };

        }


        public bool Write(string inputFile, string outputFolderMinusSlash, string connectionString)
        {
            var outputFolder = outputFolderMinusSlash + "\\";

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


                //put all types together gradually to build up list for use.  
                //tableTypes can have both built in an custom types, so they need to be 
                //unioned first.
                var customAndBuiltInTypes = builtInDataTypes.Union(customTypes);

                var tableTypes =
                    document.SelectNodes("d:DataSchemaModel/d:Model/d:Element[@Type='SqlTableType']", manager)
                        .Cast<XmlNode>()
                        .Select(selector: n => GetTableType(n, manager, customAndBuiltInTypes))
                        .ToList();

                var allTypes = customAndBuiltInTypes
                    .Cast<IType>()
                    .Union(tableTypes).ToDictionary(x => x.Name);


                var nodes = document.SelectNodes("d:DataSchemaModel/d:Model/d:Element", manager);

                var procedures = nodes.Cast<XmlNode>()
                    .Where(n => n.Attributes["Type"].Value.Equals("SqlProcedure"))
                    .Select(n => ParseProcedure(n, manager, allTypes, connection)).ToList();

                var supplementCode = new List<Tuple<string, string, string>> 
                {
                    new Tuple<string, string, string>("StructuredSight.Data", "IDatabase",
                        OutputHelpers.IDatabaseConnectionCode),
                    new Tuple<string, string, string>("StructuredSight.Data", "BaseClass",
                        OutputHelpers.BaseClass)


                };

                procedures.Union(supplementCode)
                     .ToList()
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
                             null;


            int parameterLengthValue; 

            int? parameterLength = 
                (int.TryParse(node.SelectSingleNode("d:Element/d:Relationship/d:Entry/d:Element/d:Property[@Name='Length']",
                    manager)?
                    .Attributes["Value"].Value, out parameterLengthValue)) ? (int?)parameterLengthValue : null;

            return new Field
            {
                Name = name,
                Type = types[typeName].Type,
                AllowsNull = allowsNull,
                IsOutput = isOuput,
                Length = parameterLength
            };
        }

        public Tuple<string, string, string> ParseProcedure
            (XmlNode node, XmlNamespaceManager manager, Dictionary<string,IType> types, SqlConnection connection)
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
                Type = Type.GetType("System.Int64").ToDataType(8, System.Data.SqlDbType.BigInt)
            };

            yield return new Field()
            {
                Name = "binary",
                Type = typeof(System.Byte[]).ToDataType(1, System.Data.SqlDbType.Binary)
            };

            yield return new Field()
            {
                Name = "bit",
                Type = typeof(System.Boolean).ToDataType(1, System.Data.SqlDbType.Bit)
            };

            yield return new Field()
            {
                Name = "char",
                Type = typeof(System.String).ToDataType(1, System.Data.SqlDbType.Char)
            };

            yield return new Field()
            {
                Name = "date",
                Type = typeof(System.DateTime).ToDataType(3, System.Data.SqlDbType.Date)
            };

            yield return new Field()
            {
                Name = "datetime",
                Type = typeof(System.DateTime).ToDataType(8, System.Data.SqlDbType.DateTime)
            };


            yield return new Field()
            {
                Name = "datetime2",
                Type = typeof(System.DateTime).ToDataType(8, System.Data.SqlDbType.DateTime2)
            };

            yield return new Field()
            {
                Name = "datetimeoffset",
                Type = typeof(System.DateTimeOffset).ToDataType(10, System.Data.SqlDbType.DateTimeOffset)

            };


            yield return new Field()
            {
                Name = "decimal",
                Type = typeof(System.Decimal).ToDataType(17, System.Data.SqlDbType.Decimal)
            };

            yield return new Field()
            {
                Name = "filestream",
                Type = typeof(System.Byte[]).ToDataType(1, System.Data.SqlDbType.Variant) /*?*/
            };

            yield return new Field()
            {
                Name = "float",
                Type = typeof(System.Double).ToDataType(8, System.Data.SqlDbType.Float)
            };

            yield return new Field()
            {
                Name = "image",
                Type = typeof(System.Byte[]).ToDataType(1, System.Data.SqlDbType.Image)
            };

            yield return new Field()
            {
                Name = "int",
                Type = typeof(System.Int32).ToDataType(4, System.Data.SqlDbType.Int)
            };

            yield return new Field()
            {
                Name = "money",
                Type = typeof(System.Decimal).ToDataType(8, System.Data.SqlDbType.Money)
            };

            yield return new Field()
            {
                Name = "nchar",
                Type = typeof(System.String).ToDataType(2, System.Data.SqlDbType.NChar)
            };

            yield return new Field()
            {
                Name = "ntext",
                Type = typeof(System.String).ToDataType(2, System.Data.SqlDbType.NText)
            };

            yield return new Field()
            {
                Name = "numeric",
                Type = typeof(System.Decimal).ToDataType(17, System.Data.SqlDbType.Decimal) /*?*/
            };

            yield return new Field()
            {
                Name = "nvarchar",
                Type = typeof(System.String).ToDataType(2, System.Data.SqlDbType.NVarChar)
            };

            yield return new Field()
            {
                Name = "real",
                Type = typeof(System.Single).ToDataType(4, System.Data.SqlDbType.Real)
            };

            yield return new Field()
            {
                Name = "rowversion",
                Type = typeof(System.Byte[]).ToDataType(1, System.Data.SqlDbType.Timestamp)
            };

            yield return new Field()
            {
                Name = "smalldatetime",
                Type = typeof(System.DateTime).ToDataType(4, System.Data.SqlDbType.SmallDateTime)
            };

            yield return new Field()
            {
                Name = "smallint",
                Type = typeof(System.Int16).ToDataType(2, System.Data.SqlDbType.SmallInt)
            };

            yield return new Field()
            {
                Name = "smallmoney",
                Type = typeof(System.Decimal).ToDataType(4, System.Data.SqlDbType.SmallMoney)
            };

            yield return new Field()
            {
                Name = "sql_variant",
                Type = typeof(System.Object).ToDataType(1, System.Data.SqlDbType.Variant)
            };

            yield return new Field()
            {
                Name = "text",
                Type = typeof(System.String).ToDataType(5, System.Data.SqlDbType.Text)
            };

            yield return new Field()
            {
                Name = "time",
                Type = typeof(System.TimeSpan).ToDataType(8, System.Data.SqlDbType.Time)
            };

            yield return new Field()
            {
                Name = "timestamp",
                Type = typeof(System.Byte[]).ToDataType(16, System.Data.SqlDbType.Timestamp)
            };

            yield return new Field()
            {
                Name = "tinyint",
                Type = typeof(System.Byte).ToDataType(1, System.Data.SqlDbType.TinyInt)
            };

            yield return new Field()
            {
                Name = "uniqueidentifier",
                Type = typeof(System.Guid).ToDataType(32, System.Data.SqlDbType.UniqueIdentifier)
            };

            yield return new Field()
            {
                Name = "varbinary",
                Type = typeof(System.Byte[]).ToDataType(1, System.Data.SqlDbType.VarBinary)
            };

            yield return new Field()
            {
                Name = "varchar",
                Type = typeof(System.String).ToDataType(1, System.Data.SqlDbType.VarChar)
            };

            yield return new Field()
            {
                Name = "xml",
                Type = typeof(XmlDocument).ToDataType(1, System.Data.SqlDbType.Xml)
            };
        }

    }

}

