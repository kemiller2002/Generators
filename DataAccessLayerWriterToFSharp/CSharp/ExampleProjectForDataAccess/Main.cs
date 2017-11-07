using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleProjectForDataAccess
{
    class Program
    {

        static void Main(string[] args)
        {

            using (var connection = new SqlConnection("Data Source=localhost;Initial Catalog=ExampleDatabase;Integrated Security=SSPI"))
            {
                connection.Open();
                var proc = new dbo.SelectAsOutputParameters(connection);

                var result = proc.Execute(null, null, null, null, null, null, null, null, null, null, null, null);



            }

        }


    }
}
