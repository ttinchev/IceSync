using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

#pragma warning disable MEN007

namespace IceSync.Presentation.Api.Filters
{
    /// <summary>A swagger filter to be used with Api versioning.</summary>
    public sealed class SwaggerDefaultValues : IOperationFilter
    {
        /// <inheritdoc/>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var isAuthorized = context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ||
                                 context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

            if (!isAuthorized)
            {
                return;
            }

            operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

            operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        [
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Api Key",
                                },
                            }

                        ] = new[] { string.Empty },
                    },
                };
            var apiDescription = context.ApiDescription;

            if (operation.Parameters == null)
            {
                return;
            }

            foreach (var parameter in operation.Parameters)
            {
                var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

                if (parameter.Description == null)
                {
                    parameter.Description = description.ModelMetadata?.Description;
                }

                if (parameter.Schema.Default == null && description.DefaultValue != null)
                {
                    parameter.Schema.Default = new OpenApiString(description.DefaultValue.ToString());
                }

                parameter.Required |= description.IsRequired;
            }
        }
    }
}
