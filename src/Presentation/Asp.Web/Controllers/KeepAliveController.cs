using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asp.Web.Controllers
{
    [AllowAnonymous]
    public class KeepAliveController : Controller
    {
        //
        // GET: /KeepAlive
        [AllowAnonymous]
        public ActionResult Index()
        {
            return Content("I am alive!");
        }
    }
}