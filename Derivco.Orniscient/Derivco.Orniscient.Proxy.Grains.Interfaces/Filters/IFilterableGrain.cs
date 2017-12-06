using System.Threading.Tasks;
using Derivco.Orniscient.Proxy.Grains.Models.Filters;
using Orleans;

namespace Derivco.Orniscient.Proxy.Grains.Interfaces.Filters
{
    /// <summary>
    /// Add this to a grain to give it the opportunity to return filter values for orniscient.
    /// </summary>
    public interface IFilterableGrain : IGrainWithGuidKey 
    {
        Task<FilterRow[]> GetFilters();
    }
}
