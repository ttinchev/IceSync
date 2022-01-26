using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

using IceSync.Business.Services;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IceSync
{
    /// <summary>The application entry point class.</summary>
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        /// <summary>Build web host settings.</summary>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseContentRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddEventSourceLogger();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<TimedHostedService>();
                });

        /// <summary>Main application entry point method.</summary>
        public static void Main(string[] args) =>
                CreateHostBuilder(args).Build().Run();
    }
}