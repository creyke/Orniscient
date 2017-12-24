using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Derivco.Orniscient.Proxy.Grains;
using Derivco.Orniscient.Proxy.Grains.Filters;
using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using TestProject.Grains;

namespace Derivco.Orniscient.Orleans
{
    public class OrleansClientBuilder
    {
        public async Task<IClusterClient> CreateOrleansClientAsync(ClientConfiguration configuration, int retryAttempts = 5)
        {
            var attempt = 0;
            while (true)
            {
                try
                {
                    var client = new ClientBuilder()
                        .UseConfiguration(configuration)
                        .ConfigureApplicationParts(
                            parts => parts
                                .AddFromAppDomain()
                                .AddApplicationPart(typeof(InactiveGrain).Assembly)
                                .AddApplicationPart(typeof(TypeFilterGrain).Assembly)
                                .AddApplicationPart(typeof(FilterGrain).Assembly)
                        )
                        .ConfigureLogging(logging => logging.AddConsole())
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
