using System.Threading.Tasks;
using Orleans;

namespace TestProject.Grains.Interfaces
{
	public interface IInactiveGrain : IGrainWithIntegerKey
	{
		Task DoNothing();
	}
}
