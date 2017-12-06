using System;
using System.Threading.Tasks;
using Derivco.Orniscient.Viewer.Core.Models.Connection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Rest;

namespace Derivco.Orniscient.Viewer.Core.Controllers
{
    public class ConnectionController : Controller
    {
        public Task<ViewResult> Index()
        {
            return Task.FromResult(View());
        }

        [HttpPost]
        public async Task<ActionResult> Index(ConnectionInfo connection)
        {
            try
            {
                if (!TryValidateModel(connection))
                {
                    throw new ValidationException();
                }

                return RedirectToAction("Index", "Dashboard", connection);
            }
            catch(Exception ex)
            {
                ViewBag.Error = "Connection Unsuccessful";
                return View();
            }
        }
    }
}