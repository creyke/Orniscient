using System.Collections.Generic;

namespace Derivco.Orniscient.Proxy.Core.Grains.Filters
{
    public class GroupedFilter
    {
        public string FilterName { get; set; }
        public List<string> Values { get; set; }
    }
}