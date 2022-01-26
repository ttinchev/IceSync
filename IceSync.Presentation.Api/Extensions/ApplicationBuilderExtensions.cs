using System.Linq;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace IceSync.Presentation.Api.Extensions
{
    /// <summary>The IApplicationBuilder extension methods.</summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>Add Swagger.</summary>
        public static void UseCustomSwagger(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions.Select(d => d.GroupName))
                {
                    options.SwaggerEndpoint($"/swagger/{description}/swagger.json", description.ToUpperInvariant());
                }
            });
        }
    }
}