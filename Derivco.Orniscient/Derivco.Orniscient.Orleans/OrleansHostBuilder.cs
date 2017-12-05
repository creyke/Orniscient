using System.Globalization;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans.Hosting;
using Orleans.Runtime.Configuration;
using TestGrains.Core.Grains;
using TestGrains.Interfaces.Core.Grains;

namespace Derivco.Orniscient.Orleans
{
    public class OrleansHostBuilder
    {
        public ISiloHost Build(IConfiguration configuration)
        {
            var config = ClusterConfiguration.LocalhostPrimarySilo();
            config.LoadFromFile(".\\DevTestServerConfiguration.xml");
            config.AddMemoryStorageProvider();
            config.AddMemoryStorageProvider("Default");
            config.AddMemoryStorageProvider("PubSubStore");
            config.AddSimpleMessageStreamProvider("SMSProvider");
            config.AddSimpleMessageStreamProvider("OrniscientSMSProvider");
            //TODO: Further investigation needed for their assembly scanning logic - needed for every grain, or one grain within that namespace?
            return new SiloHostBuilder()
                .UseConfiguration(config)
                .AddApplicationPartsFromReferences(typeof(InactiveGrain).Assembly)
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Error);
                })
                .ConfigureServices(
                    services => services.AddSingleton<IConfiguration>(configuration))
                .Build();
        }
    }
}
