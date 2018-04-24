using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace Protobuff.Schemas.Fluent
{
    public static class SchemaBuilderScopeExtensions
    {
        public static SchemaBuilderScope FetchTypes(this SchemaBuilderScope scope, string assembliesPath)
        {
            return FetchTypesBy(scope, assembliesPath, (filename) => true);
        }

        public static SchemaBuilderScope FetchTypesBy(this SchemaBuilderScope scope, string assembliesPath, Func<string, bool> predicate)
        {
            var assemblyFiles = Directory.GetFiles(assembliesPath).Where(predicate);
            List<Type> types = new List<Type>();

            foreach (var assemblyFileName in assemblyFiles)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(assemblyFileName);

                    types.AddRange(assembly.DefinedTypes);
                }
                catch (Exception ex)
                {


                }

            }

            scope.Types = types;

            return scope;
        }

        public static SchemaBuilderScope BuildSchema(this SchemaBuilderScope scope)
        {

            ProtoSchemaBuilder builder = new ProtoSchemaBuilder();
            string schema = builder.BuildSchema(scope.Types);

            scope.Schema = schema;
            return scope;

        }

        public static async Task WriteSchemaAsync(this SchemaBuilderScope scope, string filePath)
        {

            using (var fileStream = new StreamWriter(filePath, true))
            {
                await fileStream.WriteAsync(scope.Schema);
                await fileStream.FlushAsync();
            }
            
        }
    }
}
