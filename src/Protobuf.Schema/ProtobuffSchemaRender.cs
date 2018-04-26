using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq;

namespace Protobuff.Schemas
{
    public class ProtobuffSchemaRender : ISchemaRender
    {


        public ProtoBuf.Meta.ProtoSyntax Sintax { get; set; }
        public ProtobuffSchemaRender()
        {
            this.Sintax = ProtoBuf.Meta.ProtoSyntax.Proto2;
        }

        public ProtobuffSchemaRender(ProtoBuf.Meta.ProtoSyntax sintax)
        {
            this.Sintax = sintax;
        }

        public string Render<T>()
        {


            string schema = ProtoBuf.Serializer.GetProto<T>(Sintax);
            return schema;

        }

        


        
        public string Render(Type type)
        {
            try
            {
                if (type.IsGenericTypeDefinition)
                    return null;

                MethodInfo method = typeof(ProtobuffSchemaRender).GetMethod("Render", new Type[] { });
                MethodInfo generic = method.MakeGenericMethod(type);
                var schema = generic.Invoke(this, null);

                return schema.ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
          
        }

        public string RenderSchemaHeader(Type type)
        {
            var schema = Render(type);
            string headers = SelectLines(schema, (line) => Regex.IsMatch(line.TrimStart(), "^(package|import|syntax)"));
            return headers;
        }

        public string RenderSchemaBody(Type type)
        {
            var schema = Render(type);

            var typeBody = SelectLines(schema, (line) => !Regex.IsMatch(line.TrimStart(), "^(package|import|syntax)"));

            return typeBody;

        }



        protected virtual string SelectLines(string schema, Func<string, bool> lineFilter)
        {
            if (string.IsNullOrEmpty(schema))
                return string.Empty;

            string[] resultSchema = schema
                            .Split(Environment.NewLine.ToCharArray())
                            .Where(lineFilter)
                            .ToArray();

            var finalSchema = String.Join(Environment.NewLine, resultSchema);

            return finalSchema;
        }
    }

   
}
