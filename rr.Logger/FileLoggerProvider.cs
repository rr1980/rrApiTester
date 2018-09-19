using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using rr.LoggerBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace rr.Logger
{
    /// <summary>
    /// An <see cref="ILoggerProvider" /> that writes logs to a file
    /// </summary>
    [ProviderAlias("File")]
    public class FileLoggerProvider : BatchingLoggerProvider
    {
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
        protected override async Task WriteMessagesAsync(IEnumerable<LogMessage> messages, CancellationToken cancellationToken)
        {
            Directory.CreateDirectory(_path);

            foreach (var group in messages.GroupBy(GetGrouping))
            {
                //var fullName = GetFullName(group.Key);
                //var fileInfo = new FileInfo(fullName);
                //if (_maxFileSize > 0 && fileInfo.Exists && fileInfo.Length > _maxFileSize)
                //{
                //    return;
                //}

                var fullName = GetFileName(group.Key);

                using (var streamWriter = File.AppendText(fullName))
                {
                    foreach (var item in group)
                    {
                        Console.WriteLine(item.Message);
                        Debug.WriteLine(item.Message);
                        await streamWriter.WriteAsync(item.Message + Environment.NewLine);
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

        private (int Year, int Month, int Day) GetGrouping(LogMessage message)
        {
            return (message.Timestamp.Year, message.Timestamp.Month, message.Timestamp.Day);
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
    }
}
