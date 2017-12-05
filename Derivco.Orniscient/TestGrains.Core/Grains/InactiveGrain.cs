using System.Threading.Tasks;
using Derivco.Orniscient.Proxy.Core.Attributes;
using Orleans;
using TestGrains.Interfaces.Core.Grains;

namespace TestGrains.Core.Grains
{
	public class InactiveGrain: Grain, IInactiveGrain
	{
		[OrniscientMethod]
		public Task DoNothing()
		{
			return Task.CompletedTask;
		}
	}
}
