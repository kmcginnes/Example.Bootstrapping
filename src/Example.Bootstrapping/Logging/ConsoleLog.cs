using System;

namespace Example.Bootstrapping
{
    /// <summary>
    /// A logger that writes to Console.Out.
    /// </summary>
    public class ConsoleLog : DefaultBaseLog<ConsoleLog>
    {
        protected override void Write(string level, string message, Exception exception = null)
        {
            try
            {
                var formattedMessage = FormatMessage(level, message, exception);

                var consoleColor = GetColorForLevel(level);
                Console.ForegroundColor = consoleColor;
                Console.WriteLine(formattedMessage);
            }
            finally
            {
                Console.ResetColor();
            }
        }

        private ConsoleColor GetColorForLevel(string level)
        {
            if (String.Equals(level, "ERROR", StringComparison.InvariantCultureIgnoreCase))
            {
                return ConsoleColor.Red;
            }
            else if (String.Equals(level, "DEBUG", StringComparison.InvariantCultureIgnoreCase))
            {
                return ConsoleColor.Gray;
            }
            else if (String.Equals(level, "INFO", StringComparison.InvariantCultureIgnoreCase))
            {
                return ConsoleColor.White;
            }
            else if (String.Equals(level, "WARN", StringComparison.InvariantCultureIgnoreCase))
            {
                return ConsoleColor.Yellow;
            }
            else
            {
                return ConsoleColor.Gray;
            }
        }
    }
}