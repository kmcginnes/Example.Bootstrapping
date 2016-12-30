using System.Threading.Tasks;

namespace Example.Bootstrapping
{
    public interface ILongRunningService
    {
        Task Initialize();
    }
}