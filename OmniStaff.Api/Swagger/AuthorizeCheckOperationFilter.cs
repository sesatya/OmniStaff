using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace OmniStaff.Api.Swagger;

public class AuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAnonymous = context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any()
            || context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any();

        if (hasAnonymous)
        {
            return; // skip adding auth for anonymous endpoints (e.g., AuthController)
        }

        // Add the security requirement to the operation
        operation.Security ??= new List<OpenApiSecurityRequirement>();

        var bearerScheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
        };

        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [bearerScheme] = new string[] { }
        });
    }
}
