using System.Threading.Tasks;
using Derivco.Orniscient.Proxy.Core.Filters;
using Orleans;

namespace Derivco.Orniscient.Proxy.Core.Grains
{
    public interface ITypeFilterGrain : IGrainWithStringKey
    {
        Task RegisterFilter(string typeName, string grainId, FilterRow[] filters);
    }
}