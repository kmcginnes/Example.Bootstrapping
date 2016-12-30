using System;
using System.Threading.Tasks;

namespace Example.Bootstrapping
{
    public class QuitConsoleCommandProcessor : IConsoleCommandProcessor
    {
        public char InputCharacter => 'q';
        public string LongName => "Quit app";

        public Task Execute()
        {
            this.Log().Info("Executing Quit");
            Environment.Exit(0);
            return Task.FromResult(true);
        }
    }
}