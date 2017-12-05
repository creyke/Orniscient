using System.Threading.Tasks;
using Derivco.Orniscient.Proxy.Core.Filters;
using Orleans;

namespace Derivco.Orniscient.Proxy.Core.Grains.Filters
{
    /// <summary>
    /// Add this to a grain to give it the opportunity to return filter values for orniscient.
    /// </summary>
    public interface IFilterableGrain : IGrainWithGuidKey 
    {
        Task<FilterRow[]> GetFilters();
    }
}
