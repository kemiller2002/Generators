using System;

namespace DataAccessLayerWriter
{
    public class Field
    {
        public String Name { get; set; }
        public System.Type Type { get; set; }
        public Boolean AllowsNull { get; set; }

        public int? Length { get; set; }
    }
}