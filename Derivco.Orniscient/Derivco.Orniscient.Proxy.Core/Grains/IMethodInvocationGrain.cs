using System.Collections.Generic;
using System.Threading.Tasks;
using Derivco.Orniscient.Proxy.Core.Grains.Models;
using Orleans;

namespace Derivco.Orniscient.Proxy.Core.Grains
{
    public interface IMethodInvocationGrain : IGrainWithStringKey
    {
        Task<List<GrainMethod>> GetAvailableMethods();
        Task<string> GetGrainKeyType();
        Task<object> InvokeGrainMethod(string id, string methodId, string parametersJson, bool invokeOnNewGrain = false);
    }
}