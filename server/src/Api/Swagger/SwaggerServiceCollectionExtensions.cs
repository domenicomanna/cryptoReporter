using Microsoft.OpenApi.Models;

namespace Api.Swagger;

public static class SwaggerServiceCollectionExtensions
{
    public static void AddSwagger(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSwaggerGen(options =>
        {
            options.SupportNonNullableReferenceTypes();
            options.DescribeAllParametersInCamelCase();
            options.OperationFilter<AuthOperationFilter>();
            options.SchemaFilter<RequireNonNullablePropertiesSchemaFilter>();
            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Scheme = "bearer",
                    Description = "Enter JWT token"
                }
            );
            options.CustomOperationIds(apiDescription =>
            {
                string methodName = apiDescription.ActionDescriptor.RouteValues["action"] ?? "";
                return char.ToLower(methodName[0]) + methodName.Substring(1);
            });
        });
    }
}
