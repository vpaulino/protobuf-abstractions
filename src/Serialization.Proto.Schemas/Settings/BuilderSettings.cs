using System;
using System.Collections.Generic;
using System.Text;
using Serialization.Proto.Schemas.Transformers;

namespace Serialization.Proto.Schemas.Settings
{
    public class BuilderSettings
    {
        public BuilderSettings()
        {
            this.Rules = new List<BuilderRule>();
            this.BodyMessagesTransformers = new HashSet<ITransformer>();
            this.BodyMessagesTransformers.Add(new SplitByMessagesTransformer());
            this.HeadersTransformers = new HashSet<ITransformer>();
            
        }

        public HashSet<ITransformer>  BodyMessagesTransformers { get; set; }
        public HashSet<ITransformer>  HeadersTransformers { get; set; }
        public ICollection<BuilderRule> Rules { get; set; }
    }
}
