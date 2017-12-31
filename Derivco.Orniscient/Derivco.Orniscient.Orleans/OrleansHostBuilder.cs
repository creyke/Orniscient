using Derivco.Orniscient.Proxy.Grains;
using Derivco.Orniscient.Proxy.Grains.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans.Hosting;
using Orleans.Configuration;
using Orleans;
using Orleans.Runtime.Configuration;
using TestProject.Grains;
using Derivco.Orniscient.Proxy.Interceptors;

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
            return new SiloHostBuilder()
                .UseConfiguration(config)
                .ConfigureApplicationParts(
                    parts => parts
                    
                        .AddFromAppDomain()
                        .AddApplicationPart(typeof(InactiveGrain).Assembly)
                        .AddApplicationPart(typeof(TypeFilterGrain).Assembly)
                        .AddApplicationPart(typeof(FilterGrain).Assembly)
                )
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
