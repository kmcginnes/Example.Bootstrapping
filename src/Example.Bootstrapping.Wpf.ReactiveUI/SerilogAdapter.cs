using System;
using Example.Bootstrapping.Logging;
using Serilog;

namespace Example.Bootstrapping.Wpf.ReactiveUI
{
    public class SerilogAdapter : ILog
    {
        private readonly ILogger _logger;

        public SerilogAdapter(Type loggerTypeContext)
        {
            _logger = Serilog.Log.Logger.ForContext(loggerTypeContext);
        }

        public void Trace(string message) => _logger.Verbose(message);
        public void Debug(string message) => _logger.Debug(message);
        public void Info(string message) => _logger.Information(message);
        public void Warn(string message) => _logger.Warning(message);
        public void Warn(Exception exception, string message) => _logger.Warning(exception, message);
        public void Error(string message) => _logger.Error(message);
        public void Error(Exception exception, string message) => _logger.Error(exception, message);
        public void Fatal(string message) => _logger.Fatal(message);
        public void Fatal(Exception exception, string message) => _logger.Fatal(exception, message);
    }
}
