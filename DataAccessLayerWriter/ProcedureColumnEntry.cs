using System.Collections.Generic;

namespace DataAccessLayerWriter
{
    public class ProcedureColumnEntry
    {
        public int ColumnOrdinal { get; set; }
        public string Name { get; set; }
        public bool IsNullable { get; set; }
        public int SystemTypeId { get; set; }
        public string TypeName { get; set; }
        public int MaxLength { get; set; } 
    }


    public class ProcedureResult
    {
        public string SchemaName { get; set; }
        public string Name { get; set; }
        public IEnumerable<ProcedureColumnEntry> Columns { get; set; }
    }



}