using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Derivco.Orniscient.Proxy.Attributes;
using Orleans;
using Derivco.Orniscient.Proxy.Grains.Interfaces.Filters;
using Derivco.Orniscient.Proxy.Grains.Models;
using Derivco.Orniscient.Proxy.Grains.Models.Filters;
using TestProject.Grains.Interfaces;
using TestProject.Grains.Models;

namespace TestProject.Grains
{
    [OrniscientGrain(typeof(SubGrain), LinkType.SameId, "lightblue")]
    public class FooGrain : Grain, IFooGrain, IFilterableGrain 
    {
        private FilterRow[] _filters;

        public override async Task OnActivateAsync()
        {
            //creating dummy filters for testing
            var sports = new[] {"Rugby", "Soccer",  "Pool", "Darts", "Formula 1", "Horse Racing" };
            var rand = new Random();
            _filters = new[]
            {
                new FilterRow {FilterName = "Sport", Value =sports[rand.Next(0,5)]},
                new FilterRow {FilterName = "League", Value = "Some League Name"} //include the id here, just to see the difference
            };

            await base.OnActivateAsync();
        }

        [OrniscientMethod]
        public Task KeepAlive()
        {
            return Task.CompletedTask;
        }

        [OrniscientMethod]
        public Task<Dictionary<string, string>> KeepAliveOne(int? intOne, string stringOne)
        {
            Debug.WriteLine($"KeepAliveOne called on {this.GetPrimaryKeyString()}. intOne: {intOne} stringOne: {stringOne}");
            return Task.FromResult(new Dictionary<string, string> {{"keyone", this.GetPrimaryKey().ToString() }, {"keytwo", stringOne}});
        }

        [OrniscientMethod]
        public Task KeepAliveOne(int intOne, int stringOne)
        {
            Debug.WriteLine("KeepAliveOne called.");
            return Task.CompletedTask;
        }

        [OrniscientMethod]
        public Task KeepAliveTwo(ExternalParameterType externalParameterTwo)
        {
            Debug.WriteLine("KeepAliveTwo called.");
            return Task.CompletedTask;
        }

        [OrniscientMethod]
        public Task KeepAliveThree(Dictionary<string, int> dictionaryStringIntThree )
        {
            Debug.WriteLine("KeepAliveThree called.");
            return Task.CompletedTask;
        }

        [OrniscientMethod]
        public Task KeepAliveThree(Dictionary<int, int> dictionaryStringIntThree)
        {
            Debug.WriteLine("KeepAliveThree called.");
            return Task.CompletedTask;
        }

        [OrniscientMethod]
        public Task<FilterRow[]> GetFilters()
        {
            return Task.FromResult(_filters);
        }
    }
}