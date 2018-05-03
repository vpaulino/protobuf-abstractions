using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Serialization.Proto.Schemas.Settings;

namespace Serialization.Proto.Schemas
{
    public class ProtoSchemaBuilder : ISchemaBuilder
    {

        ISchemaRender schemaRender;
        BuilderSettings settings = new BuilderSettings(); 
     
        public ProtoSchemaBuilder()
        {
            schemaRender = new ProtobuffSchemaRender(ProtoBuf.Meta.ProtoSyntax.Proto3);
        }

        public ProtoSchemaBuilder(ISchemaRender schemaProducer)
        {
            this.schemaRender = schemaProducer;
        }

        public ProtoSchemaBuilder(BuilderSettings settings) : this()
        {
            this.settings = settings;

        }

        public ProtoSchemaBuilder(ISchemaRender schemaProducer, BuilderSettings settings) : this(schemaProducer)
        {
            this.settings = settings;

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

            if (ExcludeType(typeInfo))
                return;
                
            IEnumerable<string> headersByLine = ParseHeaders(typeInfo);

            IEnumerable<string> headers = ExecuteTransformers(typeInfo, headersByLine, settings.HeadersTransformers);

            Save(ref existingHeaders, headers);

            IEnumerable<string> bodyTypes = ParseTypeAndRelatedMessages(typeInfo);

            IEnumerable<string>  bodyMessages = ExecuteTransformers(typeInfo, bodyTypes, settings.BodyMessagesTransformers);
            
            Save(ref body, bodyMessages);

        }

        private IEnumerable<string> ExecuteTransformers(Type typeInfo, IEnumerable<string> tokens, IEnumerable<ITransformer> transformers)
        {
            
            IEnumerable<string> output = null;

            var transformersSelected = transformers.Where((transf) => transf.TargetType == null || transf.TargetType.Equals(typeInfo));

            if (transformersSelected.Count() == 0)
                return tokens;

            List<string> tokensProcessed = new List<string>();
            foreach (var transformersToExecute in transformersSelected)
            {
                if (transformersToExecute.TryTransform(tokens, out output))
                {
                    tokensProcessed.AddRange(output);
                }
                else
                {
                    tokensProcessed.AddRange(tokens);
                }
                
            }
           

            return tokensProcessed;
        }

         

        private bool ExcludeType(Type type)
        {
            var result = false;
            var exclusionRules = settings.Rules.Where((rule) => rule.Type == RuleType.Exclude);
            foreach (var rule in exclusionRules)
            {
              result |= rule.Predicate(type);
            }

            return result;
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

            var bodyTypes = new List<string>() { schemaRelatedTypes };

            return bodyTypes;
        }

        private void Save(ref HashSet<string> container, IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                try
                {
                    string trimmedValue = value.Trim().TrimStart().TrimEnd();

                    if (string.IsNullOrWhiteSpace(trimmedValue))
                        continue;
                    
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
          
            var schema = BuildSchema(assembly.GetTypes().Select<Type, TypeInfo>((t)=> t.GetTypeInfo()));
            return schema;
        }

       
         
    }
}
