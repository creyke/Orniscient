using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Derivco.Orniscient.Proxy.Grains;
using Derivco.Orniscient.Proxy.Grains.Filters;
using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using TestProject.Grains;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using Derivco.Orniscient.Proxy;
using Orleans.Configuration;
using Orleans.Hosting;

namespace Derivco.Orniscient.Orleans
{
    public class OrleansClientBuilder
    {
        public async Task<IClusterClient> CreateOrleansClientAsync(IEnumerable<IPEndPoint> gatewayEndPointList, int retryAttempts = 5)
        {
            var attempt = 0;
            while (true)
            {
                try
                {
                    var gatewayUriList = gatewayEndPointList.Select(x => x.ToGatewayUri()).ToList();
                    var client = new ClientBuilder()
                        .ConfigureCluster(options => options.ClusterId = "orniscientcluster")
                        .UseStaticClustering(options => ((List<Uri>)options.Gateways).AddRange(gatewayUriList))
                        .ConfigureApplicationParts(
                            parts => parts
                                .AddFromAppDomain()
                                .AddApplicationPart(typeof(InactiveGrain).Assembly).WithReferences()
                                .AddApplicationPart(typeof(TypeFilterGrain).Assembly).WithReferences()
                                .AddApplicationPart(typeof(FilterGrain).Assembly).WithReferences()
                        )
                        .ConfigureLogging(logging => logging.AddConsole())
                        .AddSimpleMessageStreamProvider("SMSProvider")
                        .AddSimpleMessageStreamProvider(StreamKeys.StreamProvider)
                        .Build();
                    await client.Connect();
                    Console.WriteLine("Successfully created, which has also connected to the silo host.");
                    return client;
                }
                catch (SiloUnavailableException)
                {
                    attempt++;
                    Console.WriteLine($"Attempt {attempt} of {retryAttempts} failed to initialize the Orleans client.");
                    if (attempt > retryAttempts)
                    {
                        throw;
                    }
                    await Task.Delay(TimeSpan.FromSeconds(4));
                }
            }
        }
    }
}
