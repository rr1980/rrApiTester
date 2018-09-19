using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using rr.LoggerBase;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace rr.DebugLogger
{
    /// <summary>
    /// An <see cref="ILoggerProvider" /> that writes logs to a file
    /// </summary>
    [ProviderAlias("Debug")]
    public class DebugLoggerProvider : BatchingLoggerProvider
    {
        private static Dictionary<string, BatchingLogger> _logger = new Dictionary<string, BatchingLogger>();

        /// <summary>
        /// Creates an instance of the <see cref="DebugLoggerProvider" /> 
        /// </summary>
        /// <param name="options">The options object controlling the logger</param>
        public DebugLoggerProvider(IOptions<DebugLoggerOptions> options) : base(options)
        {
            Debug.WriteLine("+++ DebugLoggerProvider");

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


                    Debug.WriteLine(result);

                }
            }


            await Task.CompletedTask;
        }

        public override ILogger CreateNewLogger(string categoryName)
        {

            if (_logger.TryGetValue(categoryName, out var logger))
            {
                return logger;
            }
            else
            {
                var newLogger = new BatchingLogger(this, categoryName, GetLogLevel(categoryName));
                _logger.Add(categoryName, newLogger);

                return newLogger;
            }
        }

    }
}
