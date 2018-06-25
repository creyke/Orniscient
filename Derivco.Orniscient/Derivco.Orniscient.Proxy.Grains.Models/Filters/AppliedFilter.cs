﻿using System.Collections.Generic;

namespace Derivco.Orniscient.Proxy.Grains.Models.Filters
{
    public class AppliedFilter
    {
        public string[] SelectedSilos { get; set; }
        public string GrainId { get; set; }
        public List<AppliedTypeFilter> TypeFilters { get; set; }
    }
}
