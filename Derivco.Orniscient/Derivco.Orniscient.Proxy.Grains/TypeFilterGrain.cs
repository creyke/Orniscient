using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Derivco.Orniscient.Proxy.Grains.Interfaces;
using Derivco.Orniscient.Proxy.Grains.Interfaces.Filters;
using Derivco.Orniscient.Proxy.Grains.Models.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Core;
using Orleans.Runtime;

namespace Derivco.Orniscient.Proxy.Grains
{
    public class TypeFilterGrain : Grain, ITypeFilterGrain
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        public TypeFilterGrain(ILogger<TypeFilterGrain> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        internal TypeFilterGrain(IGrainIdentity identity, IGrainRuntime runtime)
            : base(identity, runtime)
        {
            OnActivateAsync();
        }

        internal List<FilterRow> Filters;

        public override Task OnActivateAsync()
        {
            Filters = new List<FilterRow>();

            var configTimerPeriods = _configuration["TypeFilterGrainTimerPeriods"];
            var timerPeriods = configTimerPeriods?.Split(',').Select(int.Parse).ToArray() ?? new[] { 0, 1 };

            RegisterTimer(SendFilters, null, TimeSpan.FromSeconds(timerPeriods[0]), TimeSpan.FromSeconds(timerPeriods[1]));
            return base.OnActivateAsync();
        }

        internal async Task SendFilters(object arg)
        {
            var filterGrain = GrainFactory.GetGrain<IFilterGrain>(Guid.Empty);
            await filterGrain.UpdateTypeFilters(this.GetPrimaryKeyString(), Filters);
            Filters.Clear();
        }

        public Task RegisterFilter(string typeName, string grainId, FilterRow[] filters)
        {
            //will push to local state, then the timer will flush the state
            _logger.Trace($"Filters Registered for Grain[{typeName},Id:{grainId}][{string.Join(",", filters.Select(p => $"{p.FilterName} : {p.Value}"))}]");
            filters.All(p =>
            {
                p.GrainId = grainId;
                return true;
            });

            Filters.AddRange(filters);
            return Task.CompletedTask;
        }
    }
}