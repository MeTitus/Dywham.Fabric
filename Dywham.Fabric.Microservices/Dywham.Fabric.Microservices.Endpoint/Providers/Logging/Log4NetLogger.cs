using System;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Logging;

namespace Dywham.Fabric.Microservices.Endpoint.Providers.Logging
{
    public class Log4NetLogger : ILogger
    {
        private readonly ILog _log;


        public Log4NetLogger(string data)
        {
            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);

                writer.Write(data);
                writer.Flush();

                stream.Position = 0;

                var repo = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));

                XmlConfigurator.Configure(repo, stream);

                _log = LogManager.GetCurrentLoggers().First();
            }
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return _log.IsFatalEnabled;
                
                case LogLevel.Debug:
                case LogLevel.Trace:
                    return _log.IsDebugEnabled;
                
                case LogLevel.Error:
                    return _log.IsErrorEnabled;
                
                case LogLevel.Information:
                    return _log.IsInfoEnabled;
                
                case LogLevel.Warning:
                    return _log.IsWarnEnabled;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message) && exception == null) return;

            switch (logLevel)
            {
                case LogLevel.Critical:
                    _log.Fatal(message);
                    break;
                
                case LogLevel.Debug:
                case LogLevel.Trace:
                    _log.Debug(message);
                    break;
                
                case LogLevel.Error:
                    _log.Error(message);
                    break;
                
                case LogLevel.Information:
                    _log.Info(message);
                    break;
                
                case LogLevel.Warning:
                    _log.Warn(message);
                    break;
                
                default:
                    _log.Warn($"Encountered unknown log level {logLevel}, writing out as Info.");
                    _log.Info(message, exception);
                    break;
            }
        }
    }
}