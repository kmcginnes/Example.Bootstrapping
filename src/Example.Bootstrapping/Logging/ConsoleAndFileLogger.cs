using System;

namespace Example.Bootstrapping.Logging
{
    public class ConsoleAndFileLogger : CompositeLog
    {
        public ConsoleAndFileLogger(Type loggerTypeContext)
            : base(new ConsoleLog(loggerTypeContext), new FileLog(loggerTypeContext))
        {
        }
    }
}