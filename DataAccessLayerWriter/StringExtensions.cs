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
        public static string Join(this IEnumerable<char> items, string separator) => String.Join(separator, items);

        static string MakeWordPascal(string word) => (String.IsNullOrEmpty(word)) 
                ? word : word[0].ToString().ToUpper() + word.Skip(1).Join("");

        static string MakeWordCamel (string word) => (String.IsNullOrEmpty(word)) 
                ? word : word[0].ToString().ToLower() + word.Skip(1).Join("");

        static string FormatStatement (string item, Func<string,string> converter) => 
            item.Replace("@", "").Split(new[] { '_', '_', '-', ' ' }, StringSplitOptions.None).Select(converter).Join(",");

        public static string ToPascalCase(this string item) => FormatStatement(item, MakeWordPascal);

        public static string ToCamelCase (this string item) => FormatStatement(item, MakeWordCamel);


    }
}
