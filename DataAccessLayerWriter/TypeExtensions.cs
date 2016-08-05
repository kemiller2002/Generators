using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayerWriter
{
    public static class TypeExtensions
    {
        public static DataType ToDataType(this Type t, int sqlDefaultSize)
        {
            return new DataType
            {
                Name = t.Name,
                IsClass = t.IsClass,
                SqlDefaultSize = sqlDefaultSize
            };
        }

    }
}
