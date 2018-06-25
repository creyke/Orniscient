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
    public class OrniscientFilterIncomingCallFilter : IIncomingGrainCallFilter
    {
        private readonly ILogger<OrniscientFilterIncomingCallFilter> _logger;
        private readonly IGrainFactory _grainFactory;
        private readonly List<string> _grainsWhereTimerWasRegistered = new List<string>();
        public OrniscientFilterIncomingCallFilter(ILogger<OrniscientFilterIncomingCallFilter> logger, IGrainFactory grainFactory)
        {
            _logger = logger;
            _grainFactory = grainFactory;

            OrniscientLinkMap.Instance.Init(_logger);
        }

        public Task Invoke(IGrainCallContext context)
        {
            if (!(context.Grain is IFilterable) ||
                _grainsWhereTimerWasRegistered.Contains(((Grain)context.Grain).IdentityString))
            {
                return context.Invoke();
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
            return context.Invoke();
        }

        private Func<object, Task> GetTimerFunc(IGrainFactory grainFactory, IAddressable grain)
        {
            var grainName = grain.GetType().FullName;
            var pKey = grain.GetPrimaryKey();
            return async o =>
            {
                var filterableGrain = grain as IFilterable;
                var result = await filterableGrain.GetFilters();
                if (result != null)
                {
                    var filterGrain = grainFactory.GetGrain<ITypeFilterGrain>(grain.GetType().FullName);

                    await filterGrain.RegisterFilter(grainName, $"{pKey}", result);

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
