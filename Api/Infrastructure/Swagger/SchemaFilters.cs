using Microsoft.OpenApi.Models;
using Nudes.Retornator.Core;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Infrastructure.Swagger;

public class SchemaFilters : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(Error))
        {
            schema.Properties["message"] = schema.Properties["description"];
            schema.Properties.Remove("description");
        }
        else if (context.Type == typeof(FieldErrors))
        {
            schema.Properties["field_name"] = new OpenApiSchema()
            {
                Type = "array",
                Items = new OpenApiSchema
                {
                    Type = "string"
                }
            };
        }
    }
}
