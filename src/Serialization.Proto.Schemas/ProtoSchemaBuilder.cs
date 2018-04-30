using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Serialization.Proto.Schemas
{
    public class ProtoSchemaBuilder : ISchemaBuilder
    {

        ISchemaRender schemaRender;

     
        public ProtoSchemaBuilder()
        {
            schemaRender = new ProtobuffSchemaRender(ProtoBuf.Meta.ProtoSyntax.Proto3);
        }

        public ProtoSchemaBuilder(ISchemaRender schemaProducer)
        {
            this.schemaRender = schemaProducer;
        }

        protected virtual string Build(HashSet<string> headers, HashSet<string> bodySchema)
        {
            HashSet<string> result = new HashSet<string>();

            result.UnionWith(headers);
            result.Add($"{Environment.NewLine+Environment.NewLine}");
            result.UnionWith(bodySchema);


            string schemaResult = String.Join(Environment.NewLine, result.AsEnumerable());

            return schemaResult;

        }

        protected virtual void ParseTypeSchema(Type typeInfo, ref HashSet<string> existingHeaders, ref HashSet<string> body)
        {
            IEnumerable<string> headersByLine = ParseHeaders(typeInfo);

            Save(ref existingHeaders, headersByLine);

            IEnumerable<string> bodyTypes = ParseTypeAndRelatedMessages(typeInfo);

            Save(ref body, bodyTypes);

        }

        private IEnumerable<string> ParseHeaders(Type typeInfo)
        {
            var typeHeader = schemaRender.RenderHeader(typeInfo);
            var headersByLine = typeHeader.Split(Environment.NewLine.ToCharArray()).Where(str => !String.IsNullOrEmpty(str));
            return headersByLine;
        }

        private IEnumerable<string> ParseTypeAndRelatedMessages(Type typeInfo)
        {
            var schemaRelatedTypes = schemaRender.RenderBody(typeInfo);
            var bodyTypes = schemaRelatedTypes.Split(new string[] { $"{Environment.NewLine}message" }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select<string, string>((stringToken) => string.IsNullOrWhiteSpace(stringToken) ? string.Empty : $"{Environment.NewLine}message {stringToken}")
                                                .Where(line => !string.IsNullOrWhiteSpace(line));
            return bodyTypes;
        }

        private void Save(ref HashSet<string> container, IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                try
                {
                    string trimmedValue = value.Trim();

                    if (string.IsNullOrWhiteSpace(trimmedValue))
                        continue;
                    //if (!existingHeaders.Contains(singleHeaderEntry))
                    container.Add(Environment.NewLine);
                   
                    container.Add(trimmedValue);
                    container.Add(Environment.NewLine);
                }
                catch (Exception ex)
                {

                    throw;
                }
                

            }
        }

        public static SchemaBuilderScope Start()
        {
            return new SchemaBuilderScope();
        }

        
        public virtual string BuildSchema(Type typeInfo)
        {
            HashSet<string> bodySchema = new HashSet<string>();
            HashSet<string> headers = new HashSet<string>();

            ParseTypeSchema(typeInfo, ref headers, ref bodySchema);

            string finalSchema = Build(headers, bodySchema);
            return finalSchema;

        }

        public virtual string BuildSchema(IEnumerable<Type> typesInfo)
        {
            var bodySchema = new HashSet<string>();
            var headers = new HashSet<string>();
 
            foreach (var typeInfo in typesInfo)
            {
                ParseTypeSchema(typeInfo, ref headers, ref bodySchema);
            }
        
             
            string finalSchema = Build(headers, bodySchema);
            return finalSchema;

        }

        public virtual string BuildSchema(Assembly assembly)
        {
            assembly.GetTypes();
            var schema = BuildSchema(assembly.GetTypes().Select<Type, TypeInfo>((t)=> t.GetTypeInfo()));
            return schema;
        }

       
         
    }
}
