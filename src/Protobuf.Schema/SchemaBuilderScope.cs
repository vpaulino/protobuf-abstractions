using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Protobuff.Schemas
{
    public class SchemaBuilderScope
    {
        public List<Type> Types { get; internal set; }
        public string Schema { get; internal set; }
    }
}
