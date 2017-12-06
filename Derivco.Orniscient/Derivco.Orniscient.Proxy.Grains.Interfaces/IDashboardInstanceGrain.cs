using System.Threading.Tasks;
using Derivco.Orniscient.Proxy.Grains.Models;
using Derivco.Orniscient.Proxy.Grains.Models.Filters;
using Orleans;

namespace Derivco.Orniscient.Proxy.Grains.Interfaces
{
    public interface IDashboardInstanceGrain : IGrainWithIntegerKey
    {
        Task<DiffModel> GetAll(AppliedFilter filter = null);
        Task<GrainType[]> GetGrainTypes();
        Task SetSummaryViewLimit(int limit);
    }
}