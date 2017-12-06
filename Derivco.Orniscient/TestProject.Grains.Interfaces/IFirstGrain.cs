using System.Threading.Tasks;
using Orleans;

namespace TestProject.Grains.Interfaces
{
    public interface IFirstGrain : IGrainWithStringKey
    {
        Task KeepAlive();

        Task MockMethod();
    }
}
