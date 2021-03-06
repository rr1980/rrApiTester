﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using rr.LoggerBase;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace rr.ConsoleLogger
{
    /// <summary>
    /// An <see cref="ILoggerProvider" /> that writes logs to a file
    /// </summary>
    [ProviderAlias("Console")]
    public class ConsoleLoggerProvider : BatchingLoggerProvider
    {
        private static ConcurrentDictionary<string, BatchingLogger> _loggers = new ConcurrentDictionary<string, BatchingLogger>();

        /// <summary>
        /// Creates an instance of the <see cref="ConsoleLoggerProvider" /> 
        /// </summary>
        /// <param name="options">The options object controlling the logger</param>
        public ConsoleLoggerProvider(IOptions<ConsoleLoggerOptions> options) : base(options)
        {
        }

        /// <inheritdoc />
        protected override async Task WriteMessagesAsync(IEnumerable<IGrouping<(int Year, int Month, int Day), LogMessage>> messages, CancellationToken cancellationToken)
        {

            foreach (var group in messages)
            {
                foreach (var item in group)
                {
                    var builder = new StringBuilder();

                    if (item.Exception != null)
                    {
                        builder.AppendLine();
                    }

                    builder.Append(item.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff zzz"));
                    builder.Append(" [");
                    builder.Append(item.LogLevel.ToString());
                    builder.Append("] ");
                    builder.Append(item.Category);
                    builder.Append(": ");
                    builder.Append(item.Message);

                    if (item.Exception != null)
                    {
                        builder.AppendLine();
                        builder.AppendLine(item.Exception.ToString());
                    }

                    var result = builder.ToString();


                    Console.WriteLine(result);

                }
            }


            await Task.CompletedTask;
        }

        public override ILogger CreateNewLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new BatchingLogger(this, name, GetLogLevel(categoryName)));
        }
    }
}
