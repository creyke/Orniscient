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
using System.Net.Sockets;
using Derivco.Orniscient.Proxy;

namespace Derivco.Orniscient.Orleans
{
    public class OrleansHostBuilder
    {
        public ISiloHost Build(IConfiguration configuration)
        {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }

            var siloPort = int.Parse(configuration["SiloPort"]);
            var gatewayPort = int.Parse(configuration["GatewayPort"]);
            var siloAddress = IPAddress.Parse(localIP);          
            return new SiloHostBuilder()
                .Configure(options => options.ClusterId = "orniscientcluster")
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
                .ConfigureServices(
                    services =>
                    {
                        services.AddSingleton(configuration);
                        services.AddIncomingGrainCallFilter<OrniscientFilterIncomingCallFilter>();
                    })
                .Build();
        }
    }
}
