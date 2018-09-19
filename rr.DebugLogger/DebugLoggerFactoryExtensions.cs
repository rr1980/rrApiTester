using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace rr.DebugLogger
{

    /// <summary>
    /// Extensions for adding the <see cref="DebugLoggerProvider" /> to the <see cref="ILoggingBuilder" />
    /// </summary>
    public static class DebugLoggerFactoryExtensions
    {
        /// <summary>
        /// Adds a file logger named 'File' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        public static ILoggingBuilder AddDebugLogger(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<ILoggerProvider, DebugLoggerProvider>();
            return builder;
        }

        /// <summary>
        /// Adds a file logger named 'File' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <param name="configure">Configure an instance of the <see cref="DebugLoggerOptions" /> to set logging options</param>
        public static ILoggingBuilder AddDebugLogger(this ILoggingBuilder builder, Action<DebugLoggerOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            builder.AddDebugLogger();
            builder.Services.Configure(configure);

            return builder;
        }
    }
}
