using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Swagger;

public class AuthOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.ActionDescriptor is not ControllerActionDescriptor descriptor)
            return;

        var customAttributes = context.ApiDescription.CustomAttributes();
        if (customAttributes.Any((a) => a is AllowAnonymousAttribute))
            return;

        // Add a security requirement if the AuthorizeAttribute is present on an endpoint or controller
        if (
            customAttributes.Any((a) => a is AuthorizeAttribute)
            || descriptor.ControllerTypeInfo.GetCustomAttribute<AuthorizeAttribute>() is not null
        )
        {
            operation.Security.Add(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new string[] { }
                    }
                }
            );
        }
    }
}
