using System.Collections.Generic;

namespace Derivco.Orniscient.Proxy.Grains.Models.Filters
{
    public class AppliedTypeFilter
    {
        public string TypeName { get; set; }
        public Dictionary<string,IEnumerable<string>> SelectedValues { get; set; }
    }
}