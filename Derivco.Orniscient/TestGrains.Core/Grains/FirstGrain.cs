using System;
using System.Linq;
using System.Threading.Tasks;
using Derivco.Orniscient.Proxy.Core.Attributes;
using Microsoft.Extensions.Configuration;
using Orleans;
using TestGrains.Interfaces.Core.Grains;
using IStreamProvider = Orleans.Streams.IStreamProvider;

namespace TestGrains.Core.Grains
{
    [OrniscientGrain]
    public class FirstGrain : Grain, IFirstGrain
    {
        private IStreamProvider _streamProvider;
        private readonly IConfiguration _configuration;
        public FirstGrain(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override async Task OnActivateAsync()
        {
            _streamProvider = GetStreamProvider("SMSProvider");

            var configTimerPeriods = _configuration["FirstGrainTimerPeriods"];
            var timerPeriods = configTimerPeriods?.Split(',').Select(int.Parse).ToArray() ?? new[] { 0, 20 };

            var configAddGrainsValue = _configuration["FirstGrainAddGrainsValue"];
            var addGrainsValue = configAddGrainsValue != null ? int.Parse(configAddGrainsValue) : 1;

            RegisterTimer(p => AddGrains(addGrainsValue) ,null, TimeSpan.FromSeconds(timerPeriods[0]), TimeSpan.FromSeconds(timerPeriods[1]));
            await base.OnActivateAsync();
        }

		[OrniscientMethod]
        public async Task KeepAlive()
		{
		    await AddGrains(5);
		}

        private async Task AddGrains(int grainCountToAdd = 10)
        {
            await GrainFactory.GetGrain<IBarGrain>(8).KeepAlive();
            await GrainFactory.GetGrain<IBarGrain>(87).KeepAlive();
            await GrainFactory.GetGrain<IBarGrain>(20).KeepAlive();

            for (var i = 0; i < grainCountToAdd; i++)
            {
                var grainId = Guid.NewGuid();
                await _streamProvider.GetStream<Guid>(grainId, "TestStream").OnNextAsync(grainId);
            }
        }

        [OrniscientMethod]
        public Task MockMethod()
        {
            return Task.CompletedTask;
        }
    }
}