﻿using Derivco.Orniscient.Proxy.Grains;
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
using System.Net.Sockets;
using Derivco.Orniscient.Proxy;

namespace Derivco.Orniscient.Orleans
{
    public class OrleansHostBuilder
    {
        public ISiloHost Build(IConfiguration configuration, bool isDocker)
        {
            var siloPort = int.Parse(configuration["SiloPort"]);
            var gatewayPort = int.Parse(configuration["GatewayPort"]);
            var siloAddress = isDocker
                ? IPAddressResolver.GetIPAddressForContainers()
                : IPAddressResolver.GetIpAddressForIIS();
            return new SiloHostBuilder()
                .Configure<ClusterOptions>(options => options.ClusterId = "orniscientCluster")
                .UseDevelopmentClustering(options => options.PrimarySiloEndpoint = new IPEndPoint(siloAddress, siloPort))
                .ConfigureEndpoints(siloAddress, siloPort, gatewayPort)
                .AddMemoryGrainStorageAsDefault()
                .AddMemoryGrainStorage("Default")
                .AddMemoryGrainStorage("PubSubStore")
                .AddSimpleMessageStreamProvider("SMSProvider")
                .AddSimpleMessageStreamProvider(StreamKeys.StreamProvider)
                .ConfigureApplicationParts(
                    parts => parts

                        .AddFromAppDomain()
                        .AddApplicationPart(typeof(InactiveGrain).Assembly).WithReferences()
                        .AddApplicationPart(typeof(TypeFilterGrain).Assembly).WithReferences()
                        .AddApplicationPart(typeof(FilterGrain).Assembly).WithReferences()
                )
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Error);
                })
                .AddIncomingGrainCallFilter<OrniscientFilterIncomingCallFilter>()
                .ConfigureServices(
                    services =>
                    {
                        services.AddSingleton(configuration);
                    })
                .Build();
        }
    }
}
