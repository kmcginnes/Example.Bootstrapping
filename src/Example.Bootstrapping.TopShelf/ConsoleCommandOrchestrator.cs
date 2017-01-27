using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.Bootstrapping.TopShelf
{
    public class ConsoleCommandOrchestrator
    {
        private readonly IEnumerable<IConsoleCommandProcessor> _processorFactories;

        public ConsoleCommandOrchestrator(IEnumerable<IConsoleCommandProcessor> processorFactories)
        {
            _processorFactories = processorFactories;
        }

        public void StartUp()
        {
            // Run this event loop on a different thread than the main thread
            Task.Run(async () =>
            {
                while (true)
                {
                    await ParseMenuCommands(_processorFactories);
                }
            });
        }

        private async Task ParseMenuCommands(IEnumerable<IConsoleCommandProcessor> commandProcessors)
        {
            this.Log().Info("");
            this.Log().Info($"Possible console commands:");
            foreach (var commandProcessor in commandProcessors)
            {
                this.Log().Info($"        '{commandProcessor.InputCharacter}' = {commandProcessor.LongName}");
            }
            this.Log().Info("");
            var input = System.Console.ReadKey(true);
            var processor = commandProcessors
                .FirstOrDefault(x => x.InputCharacter == input.KeyChar);
            if (processor != null)
            {
                await processor.Execute();
            }
            else
            {
                this.Log().Warn($"Did not find a processor for command '{input.KeyChar}'.");
            }
        }
    }
}