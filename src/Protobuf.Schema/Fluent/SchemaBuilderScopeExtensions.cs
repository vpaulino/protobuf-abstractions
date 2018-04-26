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
            return FetchTypes(scope, assembliesPath, (filename) => true);
        }

        public static SchemaBuilderScope FetchTypes(this SchemaBuilderScope scope, string assembliesPath, Func<string, bool> predicate)
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

        public static SchemaBuilderScope FetchTypes(this SchemaBuilderScope scope, Assembly assembly, Func<Type, bool> predicate)
        {

            List<Type> types = new List<Type>();

            types.AddRange(assembly.DefinedTypes.Where(predicate));

            scope.Types = types;

            return scope;
        }

        public static SchemaBuilderScope AddTypes(this SchemaBuilderScope scope, IEnumerable<Type> types, Func<Type, bool> predicate)
        {
            scope.Types.AddRange(types.Where(predicate));

            return scope;
        }



        public static SchemaBuilderScope BuildSchema(this SchemaBuilderScope scope, ProtoBuf.Meta.ProtoSyntax sintax = ProtoBuf.Meta.ProtoSyntax.Proto2)
        {
            ISchemaRender render = new ProtobuffSchemaRender(sintax);
            ProtoSchemaBuilder builder = new ProtoSchemaBuilder(render);
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
