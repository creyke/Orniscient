using System.Collections.Generic;
using System.Linq;

namespace Derivco.Orniscient.Proxy.Core.Grains.Filters
{
    public class GroupedTypeFilter
    {
        public string TypeName { get; set; }
        public string TypeNameShort => TypeName.Split('.').Last();
        public List<GroupedFilter> Filters { get; set; }
    }
}