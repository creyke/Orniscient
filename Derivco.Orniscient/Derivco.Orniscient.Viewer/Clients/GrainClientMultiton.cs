using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Derivco.Orniscient.Orleans;
using Orleans;
using Orleans.Providers.Streams.SimpleMessageStream;
using Orleans.Runtime.Configuration;

namespace Derivco.Orniscient.Viewer.Clients
{
    public static class GrainClientMultiton
    {
        private static readonly Dictionary<string, IClusterClient> _clients = new Dictionary<string, IClusterClient>();
        private static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1,1);

        public static IClusterClient GetClient(string key)
        {
            return _clients.ContainsKey(key) ? _clients[key] : null;
        }

        public static async Task<IClusterClient> GetAndConnectClient(string key)
        {
            await _semaphoreSlim.WaitAsync();
            var client = GetClient(key);
            try
            {
                if (client != null && !client.IsInitialized)
                {
                    await client.Connect();
                }
            }
            finally
            {
                _semaphoreSlim.Release();

            }
            return client;
        }

        public static async Task<string> RegisterClient(string address, int port)
        {
            var grainClientKey = Guid.NewGuid().ToString();
            _semaphoreSlim.Wait();
            try
            {
                var ipEndPointList = GetIpAddressList(address).Select(ipAddress => new IPEndPoint(ipAddress, port));
                var client = await new OrleansClientBuilder().CreateOrleansClientAsync(ipEndPointList);
                _clients.Add(grainClientKey,client);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
            return grainClientKey;
        }

        public static void RemoveClient(string key)
        {
            _semaphoreSlim.Wait();
            try
            {
                if (_clients.ContainsKey(key))
                {
                    var client = _clients[key];
                    _clients.Remove(key);
                    client.Dispose();
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private static IEnumerable<IPAddress> GetIpAddressList(string address)
        {
            IPAddress[] hostAddressList;
            if (!IPAddress.TryParse(address, out var ipAddress))
            {
                var host = Dns.GetHostEntry(address);
                hostAddressList = host.AddressList;
            }
            else
            {
                hostAddressList = new[] { ipAddress };
            }
            return hostAddressList.Where(x => x.AddressFamily == AddressFamily.InterNetwork);
        }
    }
}