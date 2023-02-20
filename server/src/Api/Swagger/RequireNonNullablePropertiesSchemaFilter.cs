using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Swagger;

public class RequireNonNullablePropertiesSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var additionalRequiredFields = schema.Properties
            .Where(x => !x.Value.Nullable && !schema.Required.Contains(x.Key))
            .Select(x => x.Key);

        foreach (string property in additionalRequiredFields)
        {
            schema.Required.Add(property);
        }
    }
}
