using System;
using System.Linq;
using System.Threading.Tasks;
using Derivco.Orniscient.Proxy.Core.Filters;
using Derivco.Orniscient.Proxy.Core.Grains.Models;
using Derivco.Orniscient.Viewer.Core.Observers;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.SignalR;

namespace Derivco.Orniscient.Viewer.Core.Hubs
{
    public class OrniscientHub : Hub
    {
        private const string GrainSessionIdTypeName = "GrainSessionId";

        public OrniscientHub()
        {
            OrniscientObserver.RegisterHub(this);
        }
        public override async Task OnConnectedAsync()
        {
            
            await Groups.AddAsync(Context.ConnectionId, "userGroup");
            var grainSessionId = Context.User.Claims.First(x => x.Type == GrainSessionIdTypeName).Value;
            await OrniscientObserver.Instance.RegisterGrainClient(grainSessionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var grainSessionId = Context.User.Claims.First(x => x.Type == GrainSessionIdTypeName).Value;
            await OrniscientObserver.Instance.UnregisterGrainClient(grainSessionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task<DiffModel> GetCurrentSnapshot(AppliedFilter filter = null)
        {
            var grainSessionId = Context.User.Claims.First(x => x.Type == GrainSessionIdTypeName).Value;
            return await OrniscientObserver.Instance.GetCurrentSnapshot(filter, grainSessionId);
        }
    }
}