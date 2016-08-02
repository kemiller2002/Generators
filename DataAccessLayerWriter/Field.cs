using System;

namespace DataAccessLayerWriter
{
    public interface IType
    {
        String Name { get; set; }
        DataType Type { get; set; }
    }

    public class Field : IType
    {
        public String Name { get; set; }
        public DataType Type { get; set; }
        public Boolean AllowsNull { get; set; }

        public int? Length { get; set; }

        public bool IsOutput { get; set; }
    }
}