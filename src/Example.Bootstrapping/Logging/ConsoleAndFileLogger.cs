namespace Example.Bootstrapping.Logging
{
    public class ConsoleAndFileLogger : CompositeLog
    {
        public ConsoleAndFileLogger() : base(new ConsoleLog(), new FileLog())
        {
            
        }
    }
}