using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Derivco.Orniscient.Proxy.Grains.Interfaces;
using Derivco.Orniscient.Proxy.Grains.Interfaces.Filters;
using Derivco.Orniscient.Proxy.Grains.Models;
using Derivco.Orniscient.Proxy.Grains.Models.Filters;
using Derivco.Orniscient.Viewer.Clients;
using Derivco.Orniscient.Viewer.Hubs;
using Derivco.Orniscient.Viewer.Models.Dashboard;
using Derivco.Orniscient.Viewer.Observers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using ConnectionInfo = Derivco.Orniscient.Viewer.Models.Connection.ConnectionInfo;

namespace Derivco.Orniscient.Viewer.Controllers
{
    public class DashboardController : Controller
    {
        private const string GrainSessionIdTypeName = "GrainSessionId";
        private const string PortTypeName = "Port";
        private const string AddressTypeName = "Address";
        private static bool _allowMethodsInvocation;
        private readonly IConfiguration _configuration;
        private readonly OrniscientObserver _observer;

        public DashboardController(IConfiguration configuration, OrniscientObserver observer)
        {
            _configuration = configuration;
            _observer = observer;
        }

        public string GrainSessionId
        {
            get
            {
                return HttpContext.User.Claims.First(x => x.Type == GrainSessionIdTypeName).Value;
            }
        }

        // GET: Dashboard
        public async Task<ViewResult> Index(ConnectionInfo connection)
        {
            try
            {
                await TryCleanupConnection(connection);

                if(!HttpContext.User.Identity.IsAuthenticated)
                {
                    var grainSessionIdKey = await GrainClientMultiton.RegisterClient(connection.Address, connection.Port);
                    var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
                        new List<Claim>
                        {
                            new Claim(AddressTypeName,connection.Address),
                            new Claim(PortTypeName, connection.Port.ToString()),
                            new Claim(GrainSessionIdTypeName, grainSessionIdKey)
                        }));
                    await HttpContext.SignInAsync(claimsPrincipal,new AuthenticationProperties{IsPersistent = true});
                    HttpContext.Response.Cookies.Append(GrainSessionIdTypeName, grainSessionIdKey);
                }

                _allowMethodsInvocation = AllowMethodsInvocation();
                ViewBag.AllowMethodsInvocation = _allowMethodsInvocation;
                return View();
            }
            catch (Exception)
            {
                return View("InitError");
            }
        }

        private async Task TryCleanupConnection(ConnectionInfo connection)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var client = GrainClientMultiton.GetClient(GrainSessionId);
                var gateway = client.Configuration.Gateways.First();
                if (gateway.Address.ToString() != connection.Address ||
                    gateway.Port != connection.Port)
                {
                    await CleanupClient();
                    await HttpContext.SignOutAsync();
                }
            }
        }

        public async Task<ActionResult> Disconnect()
        {
            await CleanupClient();
            await HttpContext.SignOutAsync();
            HttpContext.Response.Cookies.Delete(GrainSessionIdTypeName);
            return RedirectToAction("Index", "Connection");
        }

        private async Task CleanupClient()
        {
            await _observer.UnregisterGrainClient(GrainSessionId);
            GrainClientMultiton.RemoveClient(GrainSessionId);
        }

        public async Task<ActionResult> GetDashboardInfo()
		{
            var clusterClient = GrainClientMultiton.GetClient(GrainSessionId);
            var dashboardCollectorGrain = clusterClient.GetGrain<IDashboardCollectorGrain>(Guid.Empty);

            var types = await dashboardCollectorGrain.GetGrainTypes();

			var dashboardInfo = new DashboardInfo
			{
				Silos = await dashboardCollectorGrain.GetSilos(),
				AvailableTypes = types
			};

			return Json(dashboardInfo);
		}

		public async Task<ActionResult> GetFilters([FromBody]GetFiltersRequest filtersRequest)
		{
			if (filtersRequest?.Types == null)
				return Json(new List<GroupedTypeFilter>() { });

		    var clusterClient = GrainClientMultiton.GetClient(GrainSessionId);
            var filterGrain = clusterClient.GetGrain<IFilterGrain>(Guid.Empty);
			var filters = await filterGrain.GetGroupedFilterValues(filtersRequest.Types);
			return Json(filters);
		}

		[HttpPost]
		public async Task<ActionResult> GetGrainInfo([FromBody]GetGrainInfoRequest grainInfoRequest)
		{
		    var clusterClient = GrainClientMultiton.GetClient(GrainSessionId);
            var typeFilterGrain = clusterClient.GetGrain<IFilterGrain>(Guid.Empty);
			var filters = await typeFilterGrain.GetFilters(grainInfoRequest.GrainType, grainInfoRequest.GrainId);
			return Json(filters);
		}

		[HttpPost]
		public async Task SetSummaryViewLimit([FromBody]int summaryViewLimit)
		{
		    var clusterClient = GrainClientMultiton.GetClient(GrainSessionId);
            var dashboardInstanceGrain = clusterClient.GetGrain<IDashboardInstanceGrain>(0);
			await dashboardInstanceGrain.SetSummaryViewLimit(summaryViewLimit);
		}

		[HttpPost]
		public async Task<ActionResult> GetInfoForGrainType([FromBody]string type)
		{
		    var clusterClient = GrainClientMultiton.GetClient(GrainSessionId);
            var dashboardCollectorGrain = clusterClient.GetGrain<IDashboardCollectorGrain>(Guid.Empty);
			var ids = await dashboardCollectorGrain.GetGrainIdsForType(type);

            var grainInfoGrain = clusterClient.GetGrain<IMethodInvocationGrain>(type);
			var methods = new List<GrainMethod>();
			if (_allowMethodsInvocation)
			{
				methods = await grainInfoGrain.GetAvailableMethods();
			}
			var keyType = await grainInfoGrain.GetGrainKeyType();

			return Json(new {Methods = methods, Ids = ids, KeyType = keyType});
		}

		[HttpGet]
		public async Task<ActionResult> GetAllGrainTypes()
		{
		    var clusterClient = GrainClientMultiton.GetClient(GrainSessionId);
            var dashboardCollectorGrain = clusterClient.GetGrain<IDashboardCollectorGrain>(Guid.Empty);
			var types = await dashboardCollectorGrain.GetGrainTypes();
			return Json(types);
		}

		[HttpPost]
		public async Task<ActionResult> GetGrainKeyFromType([FromBody]string type)
		{
		    var clusterClient = GrainClientMultiton.GetClient(GrainSessionId);
            var grainInfoGrain = clusterClient.GetGrain<IMethodInvocationGrain>(type);
			var grainKeyType = await grainInfoGrain.GetGrainKeyType();
			return Json(grainKeyType);
		}

		[HttpPost]
		public async Task<ActionResult> InvokeGrainMethod([FromBody] InvokeGrainMethodRequest request)
		{
			if (_allowMethodsInvocation)
			{
				try
				{
				    var clusterClient = GrainClientMultiton.GetClient(GrainSessionId);
                    var methodGrain = clusterClient.GetGrain<IMethodInvocationGrain>(request.Type);
					var methodReturnData = await methodGrain.InvokeGrainMethod(request.Id, request.MethodId, request.ParametersJson);
					return Json(methodReturnData);

				}
				catch (Exception)
				{
					return new StatusCodeResult((int)HttpStatusCode.BadRequest);
				}
			}
			return new StatusCodeResult((int)HttpStatusCode.Forbidden);
		}

        private bool AllowMethodsInvocation()
		{
			bool allowMethodsInvocation;
			if (!bool.TryParse(_configuration["AllowMethodsInvocation"], out allowMethodsInvocation))
			{
				allowMethodsInvocation = true;
			}

			return allowMethodsInvocation;
		}
	}
}