using Microsoft.Extensions.Logging;
using rr.ConsoleLogger;
using rr.DebugLogger;
using System;

namespace rr.FileLogger
{

    public static class LoggerFactoryExtensions
    {
        public static ILoggingBuilder AddRRFileLogger(this ILoggingBuilder builder)
        {
            return FileLoggerFactoryExtensions.AddFileLogger(builder);
        }

        public static ILoggingBuilder AddRRFileLogger(this ILoggingBuilder builder, string filename)
        {
            return FileLoggerFactoryExtensions.AddFileLogger(builder, filename);
        }

        public static ILoggingBuilder AddRRFileLogger(this ILoggingBuilder builder, Action<FileLoggerOptions> configure)
        {
            return FileLoggerFactoryExtensions.AddFileLogger(builder, configure);
        }




        public static ILoggingBuilder AddRRDebugLogger(this ILoggingBuilder builder)
        {
            return DebugLoggerFactoryExtensions.AddDebugLogger(builder);
        }


        public static ILoggingBuilder AddRRDebugLogger(this ILoggingBuilder builder, Action<DebugLoggerOptions> configure)
        {
            return DebugLoggerFactoryExtensions.AddDebugLogger(builder, configure);
        }




        public static ILoggingBuilder AddRRConsoleLogger(this ILoggingBuilder builder)
        {
            return ConsoleLoggerFactoryExtensions.AddConsoleLogger(builder);
        }


        public static ILoggingBuilder AddRRConsoleLogger(this ILoggingBuilder builder, Action<ConsoleLoggerOptions> configure)
        {
            return ConsoleLoggerFactoryExtensions.AddConsoleLogger(builder, configure);
        }
    }
}
