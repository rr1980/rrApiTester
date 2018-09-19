using Microsoft.Extensions.Logging;
using System;

namespace rr.LoggerBase
{
    public struct LogMessage
    {
        public DateTimeOffset Timestamp { get; set; }
        public LogLevel LogLevel { get; set; }
        public string Category { get; internal set; }
        public EventId EventId { get; set; }
        public object State { get; set; }
        public Exception Exception { get; set; }
        public string Message { get; set; }
    }
}
