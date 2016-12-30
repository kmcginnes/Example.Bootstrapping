using System.Threading.Tasks;

namespace Example.Bootstrapping
{
    public class SweepKickoffConsoleCommandProcessor : IConsoleCommandProcessor
    {
        public char InputCharacter => 's';
        public string LongName => "Sweep";
        public Task Execute()
        {
            this.Log().Info("Executing Sweep Kickoff");
            return Task.FromResult(true);
        }
    }
}