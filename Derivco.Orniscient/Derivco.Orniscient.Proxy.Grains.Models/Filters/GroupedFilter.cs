using System.Collections.Generic;

namespace Derivco.Orniscient.Proxy.Grains.Models.Filters
{
    public class GroupedFilter
    {
        public string FilterName { get; set; }
        public List<string> Values { get; set; }
    }
}