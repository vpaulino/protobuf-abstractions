using System;

namespace Serialization.Proto.Schemas
{
    public interface ISchemaRender
    {
        string Render<T>();

        string Render(Type type);

        string RenderHeader(Type type);

        string RenderBody(Type type);

    }
}