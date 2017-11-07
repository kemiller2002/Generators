

            using System.Data.SqlClient;
            using System.Reflection;
            using dbo;
            using System.Linq; 
        
            namespace StructuredSight.Data
            {   

            public abstract class BaseAccess<T> : IDynamicExecute<T> {

            internal IDatabaseConnection Connection;

            public BaseAccess (IDatabaseConnection connection) {
                Connection = connection;
            }

            public BaseAccess (SqlConnection connection) {
            
                Connection = new DatabaseConnection(connection);

            }
    
            public T DynamicExecute(object properties)
            {
                var type = properties.GetType();

                var objectProperties = type.GetProperties().ToDictionary(x=>x.Name);

                var thisType = this.GetType();

                var executeMethod = thisType
                    .GetMethods().First(x => x.Name.Equals("Execute"));

                var executeParameters = executeMethod.GetParameters().OrderBy(x => x.Position)
                    .Select(x =>
                    {
                        PropertyInfo tryVal;

                        return (objectProperties.TryGetValue(x.Name, out tryVal))
                            ? tryVal.GetValue(properties)
                            : null;
                    });


                return (T) 
                       executeMethod.Invoke(this, executeParameters.ToArray());
            }

        }

}


