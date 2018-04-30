using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Serialization.Proto.Schemas
{
    public interface ISchemaBuilder
    {

        string BuildSchema(Assembly assembly);

        string BuildSchema(IEnumerable<Type> typesInfo);

        string BuildSchema(Type type);

    }
}