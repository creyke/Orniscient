﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Derivco.Orniscient.Proxy.Grains.Models;
using Derivco.Orniscient.Proxy.Grains.Models.Filters;
using Derivco.Orniscient.Viewer.Observers;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.SignalR;

namespace Derivco.Orniscient.Viewer.Hubs
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
            var httpContext = Context.Connection.GetHttpContext();
            var grainSessionId = httpContext.Request.Cookies.FirstOrDefault(x => x.Key == GrainSessionIdTypeName).Value;
            await OrniscientObserver.Instance.RegisterGrainClient(grainSessionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var httpContext = Context.Connection.GetHttpContext();
            var grainSessionId = httpContext.Request.Cookies.FirstOrDefault(x => x.Key == GrainSessionIdTypeName).Value;
            await OrniscientObserver.Instance.UnregisterGrainClient(grainSessionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task<DiffModel> GetCurrentSnapshot(AppliedFilter filter = null)
        {
            var httpContext = Context.Connection.GetHttpContext();
            var grainSessionId = httpContext.Request.Cookies.FirstOrDefault(x => x.Key == GrainSessionIdTypeName).Value;
            return await OrniscientObserver.Instance.GetCurrentSnapshot(filter, grainSessionId);
        }
    }
}