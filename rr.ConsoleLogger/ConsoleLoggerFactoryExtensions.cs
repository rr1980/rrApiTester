using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace rr.ConsoleLogger
{

    /// <summary>
    /// Extensions for adding the <see cref="ConsoleLoggerProvider" /> to the <see cref="ILoggingBuilder" />
    /// </summary>
    public static class ConsoleLoggerFactoryExtensions
    {
        /// <summary>
        /// Adds a file logger named 'File' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        public static ILoggingBuilder AddConsoleLogger(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<ILoggerProvider, ConsoleLoggerProvider>();
            return builder;
        }

        /// <summary>
        /// Adds a file logger named 'File' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <param name="configure">Configure an instance of the <see cref="DebugLoggerOptions" /> to set logging options</param>
        public static ILoggingBuilder AddConsoleLogger(this ILoggingBuilder builder, Action<ConsoleLoggerOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            builder.AddConsoleLogger();
            builder.Services.Configure(configure);

            return builder;
        }
    }
}
