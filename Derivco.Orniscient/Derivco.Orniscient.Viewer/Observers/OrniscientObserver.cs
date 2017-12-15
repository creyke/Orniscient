using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Derivco.Orniscient.Proxy;
using Derivco.Orniscient.Proxy.Grains.Interfaces;
using Derivco.Orniscient.Proxy.Grains.Models;
using Derivco.Orniscient.Proxy.Grains.Models.Filters;
using Derivco.Orniscient.Viewer.Clients;
using Derivco.Orniscient.Viewer.Hubs;
using Microsoft.AspNetCore.SignalR;
using Orleans.Streams;

namespace Derivco.Orniscient.Viewer.Observers
{
    public class OrniscientObserver : IAsyncObserver<DiffModel>
    {
        private static readonly Dictionary<string,StreamSubscriptionHandle<DiffModel>> StreamHandles = new Dictionary<string,StreamSubscriptionHandle<DiffModel>>();

        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        private readonly IHubContext<OrniscientHub> _hubContext;

        public OrniscientObserver(IHubContext<OrniscientHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task<DiffModel> GetCurrentSnapshot(AppliedFilter filter, string grainSessionId)
        {
            var clusterClient = await GrainClientMultiton.GetAndConnectClient(grainSessionId);
            var dashboardInstanceGrain = clusterClient.GetGrain<IDashboardInstanceGrain>(0);
            
            var diffmodel = await dashboardInstanceGrain.GetAll(filter);

            return diffmodel;
        }

        public Task OnNextAsync(DiffModel item, StreamSequenceToken token = null)
        {
            if (item != null)
            {
                _hubContext.Clients.Group("userGroup").InvokeAsync("grainActivationChanged", item);
            }
            return Task.CompletedTask;
        }

        public Task OnCompletedAsync()
        {
            return Task.CompletedTask;
        }

        public Task OnErrorAsync(Exception ex)
        {
            return Task.CompletedTask;
        }

        public async Task RegisterGrainClient(string grainSessionId)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                if (!StreamHandles.ContainsKey(grainSessionId))
                {
                    var clusterClient = await GrainClientMultiton.GetAndConnectClient(grainSessionId);
                    var streamprovider = clusterClient.GetStreamProvider(StreamKeys.StreamProvider);
                    var stream = streamprovider.GetStream<DiffModel>(Guid.Empty, StreamKeys.OrniscientClient);
                    StreamHandles.Add(grainSessionId, await stream.SubscribeAsync(this));
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task UnregisterGrainClient(string grainSessionId)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                if (StreamHandles.ContainsKey(grainSessionId))
                {
                    var streamHandle = StreamHandles[grainSessionId];
                    StreamHandles.Remove(grainSessionId);
                    await streamHandle.UnsubscribeAsync();
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}