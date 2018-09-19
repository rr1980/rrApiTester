using Microsoft.Extensions.Logging;
using System;

namespace rr.LoggerBase
{
    public class BatchingLogger : ILogger
    {
        private readonly BatchingLoggerProvider _provider;
        private readonly string _category;
        private readonly LogLevel _logLevel;

        public BatchingLogger(BatchingLoggerProvider loggerProvider, string categoryName, LogLevel logLevel)
        {
            _provider = loggerProvider;
            _category = categoryName;
            _logLevel = logLevel;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            if (logLevel >= _logLevel)
            {
                return true;
            }
            return false;
        }

        //public void Log<TState>(DateTimeOffset timestamp, LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        //{
        //    if (!IsEnabled(logLevel))
        //    {
        //        return;
        //    }

        //    var builder = new StringBuilder();

        //    if (exception != null)
        //    {
        //        builder.AppendLine();
        //    }

        //    builder.Append(timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff zzz"));
        //    builder.Append(" [");
        //    builder.Append(logLevel.ToString());
        //    builder.Append("] ");
        //    builder.Append(_category);
        //    builder.Append(": ");
        //    builder.Append(formatter(state, exception));

        //    if (exception != null)
        //    {
        //        builder.AppendLine();
        //        builder.AppendLine(exception.ToString());
        //    }

        //    var result = builder.ToString();

        //    _provider.AddMessage(timestamp, result);
        //}

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            //Log(DateTimeOffset.Now, logLevel, eventId, state, exception, formatter);
            if (!IsEnabled(logLevel))
            {
                return;
            }

            _provider.AddMessage(new LogMessage
            {
                Timestamp = DateTimeOffset.Now,
                Category = _category,
                EventId = eventId,
                State = state,
                Exception = exception,
                Message = formatter(state, exception)
            });

        }
    }
}
