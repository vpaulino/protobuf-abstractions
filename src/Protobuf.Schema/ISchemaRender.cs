using System;

namespace Protobuff.Schemas
{
    public interface ISchemaRender
    {
        string Render<T>();

        string Render(Type type);

        string RenderHeader(Type type);

        string RenderBody(Type type);

    }
}