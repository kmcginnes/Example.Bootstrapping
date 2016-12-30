using System.Threading.Tasks;

namespace Example.Bootstrapping
{
    public interface IConsoleCommandProcessor
    {
        char InputCharacter { get; }
        string LongName { get; }
        Task Execute();
    }
}