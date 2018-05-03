using System;
using System.Collections.Generic;

namespace Serialization.Proto.Schemas
{
    public interface ITransformer
    {
        Type TargetType { get;  }
        bool TryTransform(IEnumerable<string> input, out IEnumerable<string> output);
    }
}