using System.Threading.Tasks;
using Derivco.Orniscient.Proxy.Core.Attributes;
using Orleans;

namespace TestGrains.Interfaces.Core.Grains
{
    public interface IBarGrain : IGrainWithIntegerKey
    {
        Task KeepAlive();
    }
}
