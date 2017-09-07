using System;
using Microsoft.Extensions.Logging;

namespace Surpass.Logging
{
    [ProviderAlias("File")]
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly Func<string, LogLevel, bool> _filter;

        public FileLoggerProvider()
        {
            _filter = null;
        }

        public FileLoggerProvider(Func<string, LogLevel, bool> filter)
        {
            _filter = filter;
        }

        public ILogger CreateLogger(string name)
        {
            return new FileLogger(name, _filter);
        }

        public void Dispose()
        {
            FileLoggerWriter.Instance.CancellationToken.Cancel();
        }
    }
}
