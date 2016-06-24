﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Derivco.Orniscient.Proxy.Filters;
using Derivco.Orniscient.Proxy.Grains.Models;
using Orleans;
using Orleans.CodeGeneration;

namespace Derivco.Orniscient.Proxy.Grains.Filters
{
    public class TypeFilterGrain : Grain, ITypeFilterGrain
    {
        private List<FilterRowSummary> _filters;

        public override Task OnActivateAsync()
        {
            _filters = new List<FilterRowSummary>();
            RegisterTimer(UpdateFilters, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(20));
            return base.OnActivateAsync();
        }

        private async Task UpdateFilters(object o)
        {
            //get all grains of this type.
            var orniscientReportingGrain = GrainFactory.GetGrain<IOrniscientReportingGrain>(Guid.Empty);
            var grains = await orniscientReportingGrain.GetAll(this.GetPrimaryKeyString());

            var filterTasks = grains.Select(async model =>
            {
                IFilterableGrain filterableGrain = null;
                try
                {
                    filterableGrain = GrainFactory.GetGrain<IFilterableGrain>(model.Guid, model.Type);
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("IFilterableGrain"))
                    {
                        Console.WriteLine("Not a valid filterable grain.");
                        throw;
                    }
                    else
                    {
                        Console.WriteLine("Not a valid filterable grain.");
                    }
                }

                if (filterableGrain != null)
                {
                    var filters = await filterableGrain.GetFilters();
                    if (filters != null)
                    {
                        foreach (var row in filters)
                        {
                            var filterRow = _filters.FirstOrDefault(p => p.Name == row.Name && p.Value == row.Value);
                            if (filterRow == null)
                            {
                                _filters.Add(new FilterRowSummary(row, model.Guid.ToString()));
                            }
                            else
                            {
                                if (!filterRow.GrainsWithValue.Contains(model.Guid.ToString()))
                                {
                                    filterRow.GrainsWithValue.Add(model.Guid.ToString());
                                }
                            }
                        }
                    }
                }
            });
            await Task.WhenAll(filterTasks);
        }

        public Task<List<AggregatedFilterRow>> GetFilters()
        {
            if (_filters == null)
                return Task.FromResult<List<AggregatedFilterRow>>(null);

            var result = _filters.GroupBy(p => p.Name).Select(p => new AggregatedFilterRow()
            {
                FilterName = p.Key,
                Type = this.GetPrimaryKeyString().Split('.').LastOrDefault(),
                Values = _filters.Where(f => f.Name == p.Key).Select(f => f.Value).ToList()
            }).ToList();

            return Task.FromResult(result);
        }

        public Task<List<FilterRowSummary>> GetFiltersWithIds()
        {
            return Task.FromResult(_filters);
        }

        public Task<List<string>> GetGrainIdsForFilter(AppliedTypeFilter typeFilter)
        {
            var grainIdsToReturn = new List<string>();
            foreach (var key in typeFilter.SelectedValues.Keys)
            {
                var appliedFilter = typeFilter.SelectedValues[key];
                grainIdsToReturn.AddRange(_filters.Where(p => p.Name == key && appliedFilter.Contains(p.Value)).SelectMany(p => p.GrainsWithValue));
            }
            return Task.FromResult(grainIdsToReturn);
        }

        public Task KeepAlive()
        {
            return TaskDone.Done;
        }

        public Task<List<FilterRow>> Getfilters(string grainId)
        {
            if (_filters == null)
                return Task.FromResult<List<FilterRow>>(null);

            var filters = _filters
                .Where(p => p.GrainsWithValue.Contains(grainId))
                .Select(p => new FilterRow()
                {
                    Name = p.Name,
                    Value = p.Value
                }).ToList();

            return Task.FromResult(filters);
        }
    }
}