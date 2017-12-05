using Derivco.Orniscient.Proxy.Core.Grains.Models;

namespace Derivco.Orniscient.Viewer.Core.Models.Dashboard
{
    public class DashboardInfo
    {
        public string[] Silos { get; set; }
        public GrainType[] AvailableTypes { get; set; }
    }
}