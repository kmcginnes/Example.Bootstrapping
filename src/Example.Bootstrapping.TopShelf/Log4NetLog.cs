using log4net;
using System;
using System.Diagnostics;
using System.Runtime;
using log4net.Core;
using ILog = Example.Bootstrapping.Logging.ILog;

namespace Example.Bootstrapping.TopShelf
{
    [DebuggerDisplay("{_logger.Logger.Name}")]
    public class Log4NetLog : ILog
    {
        private readonly log4net.ILog _logger;
        
        public Log4NetLog(Type loggerName)
        {
            _logger = LogManager.GetLogger(loggerName);
        }

        public void Log(Level level, string message, Exception exception = null)
        {
            _logger.Logger.Log(typeof(Log4NetLog), level, message, exception?.Demystify());
        }

        public void Log(Level level, Func<string> message, Exception exception = null)
        {
            if(_logger.Logger.IsEnabledFor(level))
                _logger.Logger.Log(typeof(Log4NetLog), level, message(), exception);
        }

        [TargetedPatchingOptOut("Performance critical")] public void Trace(string message) => Log(Level.Trace, message);
        [TargetedPatchingOptOut("Performance critical")] public void Debug(string message) => Log(Level.Debug, message);
        [TargetedPatchingOptOut("Performance critical")] public void Info(string message) => Log(Level.Info, message);
        [TargetedPatchingOptOut("Performance critical")] public void Warn(string message) => Log(Level.Warn, message);
        [TargetedPatchingOptOut("Performance critical")] public void Warn(Exception exception, string message) => Log(Level.Warn, message, exception);
        [TargetedPatchingOptOut("Performance critical")] public void Error(string message) => Log(Level.Error, message);
        [TargetedPatchingOptOut("Performance critical")] public void Error(Exception exception, string message) => Log(Level.Error, message, exception);
        [TargetedPatchingOptOut("Performance critical")] public void Fatal(string message) => Log(Level.Fatal, message);
        [TargetedPatchingOptOut("Performance critical")] public void Fatal(Exception exception, string message) => Log(Level.Fatal, message, exception);
    }
}
