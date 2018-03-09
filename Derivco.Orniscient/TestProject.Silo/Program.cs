using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
            var isDocker = args != null && args.Contains("docker");
            StartAndInvokeSiloHost(isDocker).Wait();
        }

        private static async Task StartAndInvokeSiloHost(bool isDocker)
        {
            var hostConfiguration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())  
                .AddJsonFile("orniscientHostConfiguration.json")
                .Build();

            var clientConfiguration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("orniscientClientConfiguration.json")
                .Build();

            await new OrleansHostBuilder()
                .Build(hostConfiguration,isDocker) 
                .StartAsync();

            await GrainClientWork(clientConfiguration, isDocker);
        }

        private static async Task GrainClientWork(IConfigurationRoot configuration, bool isDocker)
        {
            var localIp = isDocker ? IPAddressResolver.GetIPAddressForContainers() : IPAddressResolver.GetIpAddressForIIS();

            var ipEndPointList = new[] {new IPEndPoint(localIp, int.Parse(configuration["Port"])) };

            using (var grainClient = await new OrleansClientBuilder().CreateOrleansClientAsync(ipEndPointList))
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
