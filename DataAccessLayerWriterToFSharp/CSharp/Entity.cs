using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayerWriter
{
    public class Entity : IType
    {
        public DataType Type { get; set; }
        public String Name { get; set; }
        public IEnumerable<Field> Fields { get; set; }
    }
}
