using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using rr.ConsoleLogger;
using rr.DebugLogger;
using rr.FileLogger;
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


                    logging.AddConsoleLogger(opts =>
                    {
                        hostingContext.Configuration.GetSection("ConsoleLoggerOptions").Bind(opts);
                    });

                    logging.AddDebugLogger(opts =>
                    {
                        hostingContext.Configuration.GetSection("DebugLoggerOptions").Bind(opts);
                    });

                    logging.AddFileLogger(opts =>
                    {
                        hostingContext.Configuration.GetSection("FileLoggingOptions").Bind(opts);
                    });
                })
                //.UseKestrel(c => c.AddServerHeader = false)
                .UseStartup<Startup>()
                .Build();
    }
}
