// ReSharper disable once CheckNamespace
namespace Example.Bootstrapping
{
    public class ConsoleAndFileLogger : CompositeLog
    {
        public ConsoleAndFileLogger() : base(new ConsoleLog(), new FileLog())
        {
            
        }
    }
}