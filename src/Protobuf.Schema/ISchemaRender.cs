using System;

namespace Protobuff.Schemas
{
    public interface ISchemaRender
    {
        string Render<T>();

        string Render(Type type);

        string RenderSchemaHeader(Type type);

        string RenderSchemaBody(Type type);

    }
}