using System;
using System.Collections.Generic;
using System.Linq;

namespace Example.Bootstrapping.Logging
{
    public abstract class CompositeLog : ILog
    {
        private readonly List<ILog> _loggers;

        protected CompositeLog(params ILog[] loggers)
        {
            _loggers = loggers.ToList();
        }

        public void InitializeFor(string loggerName)
        {
            _loggers.ForEach(x => x.InitializeFor(loggerName));
        }
        
        public void Trace(string message) => _loggers.ForEach(x => x.Trace(message));
        public void Trace(Func<string> message) => _loggers.ForEach(x => x.Trace(message));
        
        public void Debug(string message) => _loggers.ForEach(x => x.Debug(message));
        public void Debug(Func<string> message) => _loggers.ForEach(x => x.Debug(message));

        public void Info(string message) => _loggers.ForEach(x => x.Info(message));
        public void Info(Func<string> message) => _loggers.ForEach(x => x.Info(message));

        public void Warn(string message) => _loggers.ForEach(x => x.Warn(message));
        public void Warn(Func<string> message) => _loggers.ForEach(x => x.Warn(message));
        public void Warn(Exception exception, string message) => _loggers.ForEach(x => x.Warn(exception, message));
        public void Warn(Exception exception, Func<string> message) => _loggers.ForEach(x => x.Warn(exception, message));

        public void Error(string message) => _loggers.ForEach(x => x.Error(message));
        public void Error(Exception exception, string message) => _loggers.ForEach(x => x.Error(exception, message));
        public void Error(Func<string> message) => _loggers.ForEach(x => x.Error(message));
        public void Error(Exception exception, Func<string> message) => _loggers.ForEach(x => x.Error(exception, message));

        public void Fatal(string message) => _loggers.ForEach(x => x.Fatal(message));
        public void Fatal(Func<string> message) => _loggers.ForEach(x => x.Fatal(message));
        public void Fatal(Exception exception, string message) => _loggers.ForEach(x => x.Fatal(exception, message));
        public void Fatal(Exception exception, Func<string> message) => _loggers.ForEach(x => x.Fatal(exception, message));
    }
}