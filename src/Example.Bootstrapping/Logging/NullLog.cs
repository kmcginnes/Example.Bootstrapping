using System;

namespace Example.Bootstrapping.Logging
{
    /// <summary>
    /// The default logger until one is set.
    /// </summary>
    public class NullLog : DefaultBaseLog
    {
        public NullLog(string loggerName) : base(loggerName) { }

        protected override void Write(string level, string message, Exception exception = null)
        {
        }
    }
}