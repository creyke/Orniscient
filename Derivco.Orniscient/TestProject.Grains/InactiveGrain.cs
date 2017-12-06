using System.Threading.Tasks;
using Derivco.Orniscient.Proxy.Attributes;
using Orleans;

using TestProject.Grains.Interfaces;

namespace TestProject.Grains
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
