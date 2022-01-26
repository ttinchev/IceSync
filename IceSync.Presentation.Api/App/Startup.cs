using IceSync.Infrastructure.Models.Settings;
using IceSync.Presentation.Api.Configuration;
using IceSync.Presentation.Api.Extensions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IceSync
{
    /// <summary>The application startup class.</summary>
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        /// <summary>Initializes a new instance of the <see cref="Startup"/> class.</summary>
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        /// <summary>This method gets called by the runtime. Use this method to add services to the container.</summary>
        public void ConfigureServices(IServiceCollection services)
        {
            // Register service Health Checks
            services.AddHealthChecks();

            // Register Db Context
            services.AddDatabase(_configuration, _environment);

            // Register MVC controllers.
            services.AddControllers();

            // Register the API versioning.
            services.AddCustomVersioning();

            // Register the Swagger generator.
            services.AddCustomSwagger();

            // Register Authentication
            services.AddAuthenticationConfiguration(_configuration);

            // Register Authorization
            services.AddAuthorizationConfiguration();

            // Register CORS rules
            services.AddCorsConfiguration(_configuration);

            services.AddHttpClientFactory(_configuration);
            services.AddServicesConfiguration();
            services.AddRepositoriesConfiguration();
            services.AddSettingsConfiguration(_configuration);
        }

        /// <summary>This method gets called by the runtime. Use this method to configure the HTTP request pipeline.</summary>
        public void Configure(IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            var applicationSettings = _configuration.GetSection(nameof(ApplicationSettings)).Get<ApplicationSettings>();
            if (_environment.IsDevelopment())
            {
                app.UseCors("AllowAll");
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseCors("AllowClient");
                app.UseHsts();
            }

            if (applicationSettings.EnableSwagger)
            {
                app.UseCustomSwagger(provider);
            }

            // Use general exceptions middleware
            app.UseMiddleware<ExceptionMiddleware>();

            // Use only https.
            app.UseHttpsRedirection();

            // enable routing.
            app.UseRouting();

            // enable MVC controllers.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
