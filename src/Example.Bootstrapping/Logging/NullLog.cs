using System;

// ReSharper disable once CheckNamespace
namespace Example.Bootstrapping
{
    /// <summary>
    /// The default logger until one is set.
    /// </summary>
    public class NullLog : DefaultBaseLog
    {
        protected override void Write(string level, string message, Exception exception = null)
        {
        }

        protected override void WriteLazy(string level, Func<string> message, Exception exception = null)
        {
        }
    }
}