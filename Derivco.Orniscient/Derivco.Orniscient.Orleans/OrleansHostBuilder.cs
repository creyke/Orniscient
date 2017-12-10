using System.Globalization;
using System.IO;
using Derivco.Orniscient.Proxy.Grains;
using Derivco.Orniscient.Proxy.Grains.Filters;
using Derivco.Orniscient.Proxy.Interceptors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime.Configuration;
using TestProject.Grains;

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
                .AddApplicationPartsFromReferences(typeof(TypeFilterGrain).Assembly)
                .AddApplicationPartsFromReferences(typeof(FilterGrain).Assembly)
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Error);
                })
                .ConfigureServices(
                    services =>
                    {
                        services.AddSingleton(configuration);
                        services.AddGrainCallFilter<OrniscientFilterCallFilter>();
                    })
                .Build();
        }
    }
}
