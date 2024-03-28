using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Protocols;
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

            // remove "DTO" from generated schema ids
            options.CustomSchemaIds(type => DefaultSchemaIdSelector(type).Replace("DTO", string.Empty));
        });
    }

    // Default schema id function from the Swashbuckle package. This is allows us to use the default schema id
    // generation and to customize it.
    // Source: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/95cb4d370e08e54eb04cf14e7e6388ca974a686e/src/Swashbuckle.AspNetCore.SwaggerGen/SchemaGenerator/SchemaGeneratorOptions.cs#L44
    private static string DefaultSchemaIdSelector(Type modelType)
    {
        if (!modelType.IsConstructedGenericType)
        {
            return modelType.Name.Replace("[]", "Array");
        }

        var prefix = modelType
            .GetGenericArguments()
            .Select(genericArg => DefaultSchemaIdSelector(genericArg))
            .Aggregate((previous, current) => previous + current);

        return prefix + modelType.Name.Split('`').First();
    }
}
