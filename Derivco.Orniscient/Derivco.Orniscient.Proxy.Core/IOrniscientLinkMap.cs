using System;
using Derivco.Orniscient.Proxy.Core.Attributes;

namespace Derivco.Orniscient.Proxy.Core
{
    public interface IOrniscientLinkMap
    {
        OrniscientGrain GetLinkFromType(string type);
        OrniscientGrain GetLinkFromType(Type type);
    }
}