using System;
using System.IO;
using System.Net;
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

            await GrainClientWork(configuration);
        }

        private static async Task GrainClientWork(IConfigurationRoot configuration)
        {
            var ipEndPoint = new IPEndPoint(IPAddress.Parse(configuration["HostAddress"]), int.Parse(configuration["HostPort"]));
            using (var grainClient = await new OrleansClientBuilder().CreateOrleansClientAsync(new[] {ipEndPoint}))
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
