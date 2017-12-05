using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Derivco.Orniscient.Proxy.Core.Attributes;
using Orleans;
using TestGrains.Interfaces.Core.Grains;

namespace TestGrains.Core.Grains
{
    [OrniscientGrain]
    public class BarGrain : Grain, IBarGrain
    {
        public Task KeepAlive()
        {
            return Task.CompletedTask;
        }
    }
}
