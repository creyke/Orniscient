using System;
using System.Threading.Tasks;
using Derivco.Orniscient.Viewer.Models.Connection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Rest;

namespace Derivco.Orniscient.Viewer.Controllers
{
    public class ConnectionController : Controller
    {
        public Task<ViewResult> Index()
        {
            return Task.FromResult(View());
        }

        [HttpPost]
        public ActionResult Index(ConnectionInfo connection)
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