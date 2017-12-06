using System;
using Derivco.Orniscient.Proxy.Grains.Models;

namespace Derivco.Orniscient.Proxy.Attributes
{
	public class OrniscientGrainAttribute : Attribute
	{
		public OrniscientGrainAttribute(Type linkFromType = null, LinkType linkType = LinkType.SameId, string colour = "",
			Type filterGrain = null, string defaultLinkFromTypeId = "")
		{

			LinkFromType = linkFromType;
			LinkType = linkType;
			Colour = colour;
			FilterGrain = filterGrain;
			DefaultLinkFromTypeId = defaultLinkFromTypeId;
		}

		public Type LinkFromType { get; }
		public LinkType LinkType { get;}
		public string Colour { get; set; }
		public Type FilterGrain { get; set; }
		public string DefaultLinkFromTypeId { get; set; }
		public bool HasLinkFromType => LinkFromType != null;
		public IdentityTypes IdentityType { get; set; }
	}
}