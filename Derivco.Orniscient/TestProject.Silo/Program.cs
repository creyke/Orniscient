using System;
using System.IO;
using System.Threading.Tasks;
using Derivco.Orniscient.Orleans;
using Microsoft.Extensions.Configuration;
using Orleans.Runtime.Configuration;
using TestProject.Grains.Interfaces;

namespace TestProject.Silo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            StartAndInvokeSiloHost().Wait();
        }

        private static async Task StartAndInvokeSiloHost()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("orniscientHostConfiguration.json")
                .Build();

            await new OrleansHostBuilder()
                .Build(configuration)
                .StartAsync();

            await GrainClientWork();
        }

        private static async Task GrainClientWork()
        {
            var configuration = ClientConfiguration.LoadFromFile(".\\DevTestClientConfiguration.xml");
            using (var grainClient = await new OrleansClientBuilder().CreateOrleansClientAsync(configuration))
            {
                try
                {
                    var firstGrain = grainClient.GetGrain<IFirstGrain>("Hallo");
                    await firstGrain.KeepAlive();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                Console.ReadLine();
            }
        }
    }
}
