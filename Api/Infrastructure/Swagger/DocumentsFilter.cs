using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Infrastructure.Swagger
{
    public class DocumentsFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var key in swaggerDoc.Components.Schemas.Keys.ToArray())
            {
                if (key.StartsWith("ResultOf["))
                {
                    swaggerDoc.Components.Schemas.Remove(key);
                }
            }
        }
    }
}