using System.Threading.Tasks;
using Orleans;

namespace TestGrains.Interfaces.Core.Grains
{
    public interface IFirstGrain : IGrainWithStringKey
    {
        Task KeepAlive();

        Task MockMethod();
    }
}
