using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using rr.Logger;
using System;

namespace rrApiTester.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateWebHostBuilder(args).Build().Run();

            var host = BuildWebHost(args);

            ILogger logger = host.Services.GetService<ILogger<Program>>();

            try
            {
                logger.LogInformation("Starting web host");
                host.Run();
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Starting web host failed.");
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddFile(opts =>
                    {
                        hostingContext.Configuration.GetSection("FileLoggingOptions").Bind(opts);
                    });
                })
                //.UseKestrel(c => c.AddServerHeader = false)
                .UseStartup<Startup>()
                .Build();
    }
}
