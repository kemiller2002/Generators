using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayerWriter
{
    public class DataType
    {
        public String Name { get; set; }

        public bool IsClass { get; set; }

        public int SqlDefaultSize { get; set; }

    }
}
