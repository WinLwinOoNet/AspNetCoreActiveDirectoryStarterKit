using Asp.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asp.Web.Areas.Administration.Controllers
{
    [Area(Constants.Areas.Administration)]
    [Authorize(Policy = Core.Constants.RoleNames.Administrator)]
    public class DashboardController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}