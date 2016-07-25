using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayerWriter
{
    static class StringExtensions
    {
        public static string RemoveSquareBrackets(this string item)
            => item?.Replace("[", "").Replace("]", "");


        public static string Join(this IEnumerable<string> items, string separator) => String.Join(separator, items);
    }
}
