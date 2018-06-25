using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Derivco.Orniscient.Proxy.Attributes;
using Orleans;
using TestProject.Grains.Interfaces;

namespace TestProject.Grains
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
