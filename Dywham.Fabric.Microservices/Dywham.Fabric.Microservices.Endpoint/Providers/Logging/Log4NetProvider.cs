using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Dywham.Fabric.Microservices.Endpoint.Providers.Logging
{
    public class Log4NetProvider : ILoggerProvider
    {
        private readonly string _data;
        private readonly ConcurrentDictionary<string, Log4NetLogger> _loggers = new();


        public Log4NetProvider(string data)
        {
            _data = data;
        }


        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, x => new Log4NetLogger(_data));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}