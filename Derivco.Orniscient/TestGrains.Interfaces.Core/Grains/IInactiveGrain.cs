using System.Threading.Tasks;
using Orleans;

namespace TestGrains.Interfaces.Core.Grains
{
	public interface IInactiveGrain : IGrainWithIntegerKey
	{
		Task DoNothing();
	}
}
