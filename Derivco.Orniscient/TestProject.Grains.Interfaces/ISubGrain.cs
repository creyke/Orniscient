using System.Threading.Tasks;
using Derivco.Orniscient.Proxy.Grains.Interfaces.Filters;

namespace TestProject.Grains.Interfaces
{
    public interface ISubGrain : IFilterableGrain
    {
        Task SayHallo();
    }
}
