using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayerWriter
{
    public class DataTypeEntry
    {

        public static Tuple<string,string> Create(string fullName, IEnumerable<Field> fields)
        {
            var nameParts = fullName.Split('.');
            var classNamespace = nameParts.Take(nameParts.Length - 1).Join(".");
            var name = nameParts.Last();

            var accessors =
                fields.Select(x => $"public {x.Type.Name} {x.Name.Split('.').Last()} {{get;set;}}").Join(System.Environment.NewLine);



            return new Tuple<string,string> (fullName,$@"
using System;
namespace {classNamespace}
{{
    public class {name}
    {{
        {accessors}
    }}
}}
            ");

         }
    }
}
