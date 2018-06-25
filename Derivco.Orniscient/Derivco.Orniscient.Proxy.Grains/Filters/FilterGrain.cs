using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Derivco.Orniscient.Proxy.Grains.Interfaces.Filters;
using Derivco.Orniscient.Proxy.Grains.Models.Filters;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Core;
using Orleans.Runtime;

namespace Derivco.Orniscient.Proxy.Grains.Filters
{
    public class FilterGrain : Grain, IFilterGrain
    {
		private readonly ILogger _logger;
		private List<TypeFilter> _filters;

		public FilterGrain(ILogger<FilterGrain> logger)
	    {
            _logger = logger;
	    }

	    internal FilterGrain(ILogger<FilterGrain> logger, IGrainIdentity identity, IGrainRuntime runtime) : base(identity, runtime)
	    {
            _logger = logger;
        }
		

        public override async Task OnActivateAsync()
        {
            _filters = new List<TypeFilter>();
            await base.OnActivateAsync();
        }

        public Task<List<TypeFilter>> GetFilters(string[] types)
        {
            return Task.FromResult(_filters);
        }

        public Task<List<GroupedTypeFilter>> GetGroupedFilterValues(string[] types)
        {
            var result = new List<GroupedTypeFilter>();

            foreach (var type in types)
            {
                var currentTypeFilter = _filters.FirstOrDefault(p => p.TypeName == type);
                if (currentTypeFilter != null)
                {
                    result.Add(new GroupedTypeFilter
                    {
                        TypeName = type,
                        Filters = currentTypeFilter.Filters
                            .GroupBy(p => p.FilterName)
                            .Select(g => new GroupedFilter
	                        {
                                FilterName = g.Key,
                                Values = currentTypeFilter.Filters.Where(f => f.FilterName == g.Key).Select(s => s.Value).Distinct().ToList()
                            }).ToList()
                    });
                }
            }
            return Task.FromResult(result);
        }

        public Task<TypeFilter> GetFilter(string type)
        {
            return Task.FromResult(_filters.FirstOrDefault(p => p.TypeName == type));
        }

        public Task KeepAlive()
        {
            return Task.CompletedTask;
        }

        public Task<List<FilterRow>> GetFilters(string type, string grainId)
        {
            var typefilter = _filters.FirstOrDefault(p => p.TypeName == type);
            return typefilter != null ? Task.FromResult(typefilter.Filters.Where(p => p.GrainId == grainId).ToList()) : Task.FromResult<List<FilterRow>>(null);
        }

        public Task UpdateTypeFilters(string typeName, IEnumerable<FilterRow> filters)
        {
            _logger.Trace($"Filters Registered for Type Grain[{typeName}, Count : {filters.Count()}]");
            var typeFilter = _filters.FirstOrDefault(p => p.TypeName == typeName);
            if (typeFilter != null)
            {
                foreach (var filterRow in filters)
                {
                    var existingFilter = typeFilter.Filters.FirstOrDefault(p => p.FilterName == filterRow.FilterName && p.GrainId==filterRow.GrainId);
                    if (existingFilter != null)
                    {
                        existingFilter.Value = filterRow.Value;
                    }
                    else
                    {
                        typeFilter.Filters.Add(filterRow);
                    }
                }
            }
            else
            {
                _filters.Add(new TypeFilter { TypeName = typeName, Filters = filters.ToList() });
            }
            return Task.CompletedTask;
        }
    }
}