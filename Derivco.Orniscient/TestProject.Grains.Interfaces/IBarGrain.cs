using System.Threading.Tasks;
using Orleans;

namespace TestProject.Grains.Interfaces
{
    public interface IBarGrain : IGrainWithIntegerKey
    {
        Task KeepAlive();
    }
}
