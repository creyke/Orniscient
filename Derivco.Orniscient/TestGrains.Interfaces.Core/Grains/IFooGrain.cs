using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using TestGrains.Model.Core;

namespace TestGrains.Interfaces.Core.Grains
{
    public interface IFooGrain : IGrainWithGuidKey
    {
        Task KeepAlive();

        Task<Dictionary<string, string>> KeepAliveOne(int? intOne, string stringOne);

        Task KeepAliveTwo(ExternalParameterType externalParameterTwo);

        Task KeepAliveThree(Dictionary<string, int> dictionaryStringIntThree);

        Task KeepAliveThree(Dictionary<int, int> dictionaryStringIntThree);
    }
}
