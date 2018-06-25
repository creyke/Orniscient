using System;
using Derivco.Orniscient.Proxy.Attributes;

namespace Derivco.Orniscient.Proxy
{
    public interface IOrniscientLinkMap
    {
        OrniscientGrainAttribute GetLinkFromType(string type);
        OrniscientGrainAttribute GetLinkFromType(Type type);
    }
}