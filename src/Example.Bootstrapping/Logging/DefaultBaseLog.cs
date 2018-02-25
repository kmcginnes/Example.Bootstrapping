using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Example.Bootstrapping.Logging
{
    [DebuggerDisplay("{" + nameof(_loggerName) + "}")]
    public abstract class DefaultBaseLog : ILog
    {
        private readonly string _loggerName;

        protected DefaultBaseLog(string loggerName)
        {
            _loggerName = loggerName;
        }

        protected string FormatMessage(string level, string message, Exception exception = null)
        {
            var timestamp = DateTime.Now.ToString("hh:mm:ss.fff tt");
            var threadName = Thread.CurrentThread.Name;
            if (String.IsNullOrEmpty(threadName))
                threadName = Thread.CurrentThread.ManagedThreadId.ToString();

            var shortLoggerName = _loggerName.Split('.').LastOrDefault() ?? "None";

            var prefix = $"{timestamp} {level,5} [{threadName}][{shortLoggerName}]";

            var result = $"{prefix}  {message}";

            if (exception != null)
            {
                result = result +
                         $"{Environment.NewLine}{exception.GetType().FullName}: {exception.Message} {exception.StackTrace}";
            }
            return result;
        }

        protected abstract void Write(string level, string message, Exception exception = null);

        public void Trace(string message) => Write("TRACE", message);
        public void Debug(string message) => Write("DEBUG", message);
        public void Info(string message) => Write("INFO", message);
        public void Warn(string message) => Write("WARN", message);
        public void Warn(Exception exception, string message) => Write("WARN", message, exception);
        public void Error(string message) => Write("ERROR", message);
        public void Error(Exception exception, string message) => Write("ERROR", message, exception);
        public void Fatal(string message) => Write("FATAL", message);
        public void Fatal(Exception exception, string message) => Write("FATAL", message, exception);
    }
}