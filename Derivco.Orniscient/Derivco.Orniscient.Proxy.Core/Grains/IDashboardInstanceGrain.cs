using System.Threading.Tasks;
using Derivco.Orniscient.Proxy.Core.Filters;
using Derivco.Orniscient.Proxy.Core.Grains.Models;
using Orleans;

namespace Derivco.Orniscient.Proxy.Core.Grains
{
    public interface IDashboardInstanceGrain : IGrainWithIntegerKey
    {
        Task<DiffModel> GetAll(AppliedFilter filter = null);
        Task<GrainType[]> GetGrainTypes();
        Task SetSummaryViewLimit(int limit);
    }
}