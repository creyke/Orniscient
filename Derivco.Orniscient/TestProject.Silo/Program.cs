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
            StartAndInvokeSiloHost().Wait();
        }

        private static async Task StartAndInvokeSiloHost()
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
                .Build(hostConfiguration) 
                .StartAsync();

            await GrainClientWork(clientConfiguration);
        }

        private static async Task GrainClientWork(IConfigurationRoot configuration)
        {
            IPAddress[] hostAddressList;
            var hostAddress = configuration["HostAddress"];
            if (!IPAddress.TryParse(hostAddress, out var ipAddress))
            {
                var host = Dns.GetHostEntry(hostAddress);
                hostAddressList = host.AddressList;
            }
            else
            {
                hostAddressList = new[] { ipAddress };
            }
            var ipEndPointList = hostAddressList.Where(x => x.AddressFamily == AddressFamily.InterNetwork).Select(address => new IPEndPoint(address, int.Parse(configuration["Port"])));
         
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
