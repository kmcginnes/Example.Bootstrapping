using System.Threading.Tasks;
using Topshelf;

namespace Example.Bootstrapping.TopShelf
{
    class QuitTopShelfServiceCommandProcessor : IConsoleCommandProcessor
    {
        private readonly HostControl _hostControl;

        public char InputCharacter => 'q';
        public string LongName => "Quit app";

        public QuitTopShelfServiceCommandProcessor(HostControl hostControl)
        {
            _hostControl = hostControl;
        }

        public Task Execute()
        {
            this.Log().Info("Stopping TopShelf host control...");
            _hostControl.Stop();
            return Task.FromResult(true);
        }
    }
}
