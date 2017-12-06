using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Derivco.Orniscient.Proxy.Grains.Interfaces;
using Derivco.Orniscient.Proxy.Grains.Interfaces.Filters;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.CodeGeneration;
using Orleans.Providers;
using Orleans.Runtime;

namespace Derivco.Orniscient.Proxy.Interceptors
{
    public class OrniscientFilterCallFilter : IGrainCallFilter
    {
        private readonly ILogger<OrniscientFilterCallFilter> _logger;
        private readonly IGrainFactory _grainFactory;
        private readonly List<string> _grainsWhereTimerWasRegistered = new List<string>();
        public OrniscientFilterCallFilter(ILogger<OrniscientFilterCallFilter> logger, IGrainFactory grainFactory)
        {
            _logger = logger;
            _grainFactory = grainFactory;
        }
        public async Task Invoke(IGrainCallContext context)
        {
            if (!(context.Grain is IFilterableGrain) ||
                _grainsWhereTimerWasRegistered.Contains(((Grain)context.Grain).IdentityString))
            {
                await context.Invoke();
            }
            var grainType = context.Grain.GetType();
            var dynamicMethod = grainType.GetMethod("RegisterTimer",
                BindingFlags.Instance | BindingFlags.NonPublic);
            dynamicMethod.Invoke(context.Grain, new object[]
            {
                GetTimerFunc(_grainFactory,context.Grain),
                null,
                TimeSpan.FromSeconds(0),
                TimeSpan.FromSeconds(20)
            });

            _grainsWhereTimerWasRegistered.Add(((Grain)context.Grain).IdentityString);
            _logger.LogInformation($"Currently we have {_grainsWhereTimerWasRegistered.Count} grains where timer was registered");
            await context.Invoke();
        }

        private Func<object, Task> GetTimerFunc(IGrainFactory grainFactory, IAddressable grain)
        {
            var grainName = grain.GetType().FullName;
            return async o =>
            {
                var filterableGrain = grain.AsReference<IFilterableGrain>();
                var result = await filterableGrain.GetFilters();
                if (result != null)
                {
                    var filterGrain = grainFactory.GetGrain<ITypeFilterGrain>(grain.GetType().FullName);
                    await filterGrain.RegisterFilter(grainName, filterableGrain.GetPrimaryKey().ToString(), result);

                    var filterString = string.Join(",", result.Select(p => $"{p.FilterName} : {p.Value}"));
                    _logger.LogInformation($"Filters for grain [Type : {grainName}] [Id : {grain.GetPrimaryKey()}][Filter : {filterString}]");
                }
                else
                {
                    _logger.LogInformation("Filter was not set yet");
                }
            };
        }
    }
}
