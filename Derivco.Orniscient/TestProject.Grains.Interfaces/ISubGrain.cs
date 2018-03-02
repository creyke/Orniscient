using System.Threading.Tasks;
using Derivco.Orniscient.Proxy.Grains.Interfaces.Filters;
using Orleans;

namespace TestProject.Grains.Interfaces
{
    public interface ISubGrain : IGrainWithGuidKey, IFilterable
    {
        Task SayHallo();
    }
}
