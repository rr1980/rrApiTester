using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using rr.LoggerBase;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace rr.FileLogger
{
    /// <summary>
    /// An <see cref="ILoggerProvider" /> that writes logs to a file
    /// </summary>
    [ProviderAlias("File")]
    public class FileLoggerProvider : BatchingLoggerProvider
    {
        private static Dictionary<string, BatchingLogger> _logger = new Dictionary<string, BatchingLogger>();

        private readonly string _path;
        private readonly string _fileName;
        private readonly int? _maxFileSize;
        private readonly int? _maxRetainedFiles;

        /// <summary>
        /// Creates an instance of the <see cref="FileLoggerProvider" /> 
        /// </summary>
        /// <param name="options">The options object controlling the logger</param>
        public FileLoggerProvider(IOptions<FileLoggerOptions> options) : base(options)
        {
            var loggerOptions = options.Value;
            _path = loggerOptions.LogDirectory;
            _fileName = loggerOptions.FileName;
            _maxFileSize = loggerOptions.FileSizeLimit;
            _maxRetainedFiles = loggerOptions.RetainedFileCountLimit;
        }

        /// <inheritdoc />
        protected override async Task WriteMessagesAsync(IEnumerable<IGrouping<(int Year, int Month, int Day), LogMessage>> messages, CancellationToken cancellationToken)
        {
            Directory.CreateDirectory(_path);

            foreach (var group in messages)
            {
                var fullName = GetFileName(group.Key);

                using (var streamWriter = File.AppendText(fullName))
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

                        builder.AppendLine();


                        await streamWriter.WriteAsync(builder.ToString());
                    }
                }
            }

            RollFiles();
        }

        private string GetFileName((int Year, int Month, int Day) group, int index = 0)
        {
            var fullName = GetFullName(group) + "(" + index + ")";

            var fileInfo = new FileInfo(fullName + ".txt");

            if (_maxFileSize > 0 && fileInfo.Exists && fileInfo.Length > _maxFileSize)
            {
                return GetFileName(group, (index + 1));
            }

            return fullName + ".txt";
        }

        private string GetFullName((int Year, int Month, int Day) group)
        {
            return Path.Combine(_path, $"{group.Year:0000}-{group.Month:00}-{group.Day:00}-{_fileName}");
            //return Path.Combine(_path, $"{_fileName}{group.Year:0000}{group.Month:00}{group.Day:00}.txt");
        }



        /// <summary>
        /// Deletes old log files, keeping a number of files defined by <see cref="FileLoggerOptions.RetainedFileCountLimit" />
        /// </summary>
        protected void RollFiles()
        {
            if (_maxRetainedFiles > 0)
            {
                var files = new DirectoryInfo(_path)
                    .GetFiles("*" + _fileName + "*")
                    .OrderByDescending(f => f.CreationTime)
                    .Skip(_maxRetainedFiles.Value);

                foreach (var item in files)
                {
                    item.Delete();
                }
            }
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
