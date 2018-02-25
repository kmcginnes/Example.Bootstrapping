namespace Example.Bootstrapping.Logging
{
    public class ConsoleAndFileLogger : CompositeLog
    {
        public ConsoleAndFileLogger(string loggerName) : base(new ConsoleLog(loggerName), new FileLog(loggerName))
        {
        }
    }
}