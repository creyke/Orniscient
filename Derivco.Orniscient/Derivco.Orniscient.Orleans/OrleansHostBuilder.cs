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
using System.Net;

namespace Derivco.Orniscient.Orleans
{
    public class OrleansHostBuilder
    {
        public ISiloHost Build(IConfiguration configuration)
        {
            var siloPort = 11111;
            int gatewayPort = 30000;
            var siloAddress = IPAddress.Loopback;          
            return new SiloHostBuilder()
                .Configure(options => options.ClusterId = "orniscientcluster")
                .UseDevelopmentClustering(options => options.PrimarySiloEndpoint = new IPEndPoint(siloAddress, siloPort))
                .ConfigureEndpoints(siloAddress, siloPort, gatewayPort)
                .AddMemoryGrainStorageAsDefault()
                .AddMemoryGrainStorage("Default")
                .AddMemoryGrainStorage("PubSubStore")
                .AddSimpleMessageStreamProvider("SMSProvider")
                .AddSimpleMessageStreamProvider("OrniscientSMSProvider")
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
                        services.AddIncomingGrainCallFilter<OrniscientFilterCallFilter>();
                    })
                .Build();
        }
    }
}
