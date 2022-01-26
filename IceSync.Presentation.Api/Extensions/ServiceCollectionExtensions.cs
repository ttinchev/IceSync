using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using IceSync.Business.Services;
using IceSync.Data;
using IceSync.Data.Repositories;
using IceSync.Infrastructure.Models.Settings;
using IceSync.Infrastructure.Repositories;
using IceSync.Infrastructure.Services;
using IceSync.Presentation.Api.Filters;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace IceSync.Presentation.Api.Extensions
{
    /// <summary>The IServiceCollection extension methods.</summary>
    public static class ServiceCollectionExtensions
    {
        private const string DefaultConnectionKey = "DefaultConnection";

        /// <summary>Add the default database.</summary>
        public static void AddDatabase(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            services.AddDbContext<IceSyncContext>(options =>
            {
                if (environment.IsDevelopment())
                {
                    options
                        .UseInMemoryDatabase("IceSyncDb")
                        .LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors();
                }
                else
                {
                    var connectionString = configuration.GetConnectionString(DefaultConnectionKey);

                    options.UseSqlServer(
                        connectionString,
                        sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly("IceSync.Data");
                        });
                }
            });
        }

        /// <summary>Configure api versioning.</summary>
        public static void AddCustomVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddVersionedApiExplorer(
                options =>
                {
                    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                    // note: the specified format code will format the version as "'v'major[.minor][-status]"
                    options.GroupNameFormat = "'v'VVV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;
                });
        }

        /// <summary>Configure dependency injection for Services.</summary>
        public static void AddServicesConfiguration(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHostedService<TimedHostedService>();

            services.AddTransient<IWorkflowService, WorkflowService>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
        }

        /// <summary>Configure dependency injection for Repositories.</summary>
        public static void AddRepositoriesConfiguration(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        }

        /// <summary>Configure authentication.</summary>
        public static void AddAuthenticationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var authenticationSettings = configuration.GetSection(nameof(ApplicationSettings)).Get<ApplicationSettings>();
            services.AddAuthentication()
               .AddJwtBearer(cfg =>
               {
                   cfg.RequireHttpsMetadata = false;
                   cfg.SaveToken = true;

                   cfg.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtSecret)),
                       ValidateIssuer = false,
                       ValidateAudience = false,
                       ValidateLifetime = true,
                   };
               });
        }

        /// <summary>Configure CORS.</summary>
        public static void AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var applicationSettings = configuration.GetSection(nameof(ApplicationSettings)).Get<ApplicationSettings>();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowClient", policy =>
                {
                    policy.WithOrigins(applicationSettings.ClientLocation)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });
        }

        /// <summary>Configure Swagger.</summary>
        public static void AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                // note: need a temporary service provider here because one has not been created yet
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var description in provider.ApiVersionDescriptions.Select(d => d.GroupName))
                {
                    options.SwaggerDoc(description, new OpenApiInfo
                    {
                        Version = description,
                        Title = "IceSync API",
                        Description = "Web API for IceSync project.",
                    });
                }

                options.AddSecurityDefinition("Api Key", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme.",
                });

                options.OperationFilter<SwaggerDefaultValues>();

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                options.IncludeXmlComments(xmlPath);
            });
        }

        /// <summary>Configure application settings.</summary>
        public static void AddSettingsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ApplicationSettings>(configuration.GetSection(nameof(ApplicationSettings)));
            services.AddSingleton<ApplicationSettings>();
        }

        /// <summary>Configure application authorization policies.</summary>
        public static void AddAuthorizationConfiguration(this IServiceCollection services)
        {
            services.AddAuthorization();
        }

        /// <summary>Configure Http Client and authentication.</summary>
        public static void AddHttpClientFactory(this IServiceCollection services, IConfiguration configuration)
        {
            var applicationSettings = configuration.GetSection(nameof(ApplicationSettings)).Get<ApplicationSettings>();

            services.AddHttpClient("WorkflowsClient", client =>
            {
                client.BaseAddress = new Uri(applicationSettings.WorkflowsAPI);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
        }
    }
}