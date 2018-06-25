using System.Threading.Tasks;
using Derivco.Orniscient.Proxy.Grains.Models.Filters;
using Orleans;

namespace Derivco.Orniscient.Proxy.Grains.Interfaces
{
    public interface ITypeFilterGrain : IGrainWithStringKey
    {
        Task RegisterFilter(string typeName, string grainId, FilterRow[] filters);
    }
}