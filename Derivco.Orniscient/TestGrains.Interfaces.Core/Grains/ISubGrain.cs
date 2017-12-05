using System.Threading.Tasks;
using Derivco.Orniscient.Proxy.Core.Grains.Filters;

namespace TestGrains.Interfaces.Core.Grains
{
    public interface ISubGrain : IFilterableGrain
    {
        Task SayHallo();
    }
}
