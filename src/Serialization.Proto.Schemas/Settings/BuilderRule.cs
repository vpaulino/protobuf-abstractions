using System;
using System.Collections.Generic;
using System.Text;

namespace Serialization.Proto.Schemas.Settings
{

    public class BuilderRule
    {
        public Func<Type, bool> Predicate { get; set; }

        public RuleType Type { get; set; }
    }
}
