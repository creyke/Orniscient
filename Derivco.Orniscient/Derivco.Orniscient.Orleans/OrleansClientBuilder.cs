﻿using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System.Threading.Tasks;
using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using TestGrains.Core.Grains;
using TestGrains.Interfaces.Core.Grains;

namespace Derivco.Orniscient.Orleans
{
    public class OrleansClientBuilder
    {
        public async Task<IClusterClient> CreateOrleansClientAsync(int retryAttempts = 5)
        {
            var attempt = 0;
            while (true)
            {
                try
                {
                    var configuration = ClientConfiguration.LoadFromFile(".\\DevTestClientConfiguration.xml");
                    //TODO: Further investigation needed for their assembly scanning logic - needed for every grain, or one grain within that namespace?
                    var client = new ClientBuilder()
                        .UseConfiguration(configuration)
                        .AddApplicationPartsFromReferences(typeof(IInactiveGrain).Assembly)
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
