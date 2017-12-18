using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Derivco.Orniscient.Viewer.Models.Dashboard
{
    public class InvokeGrainMethodRequest
    {
        public string Type;
        public string Id;
        public string MethodId;
        public string ParametersJson;
        public bool InvokeOnNewGrain;
    }
}
