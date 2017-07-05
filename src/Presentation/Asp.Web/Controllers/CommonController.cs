using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asp.Web.Controllers
{
    [AllowAnonymous]
    public class CommonController : Controller
    {
        //
        // GET: /Common/Error
        public IActionResult Error(string id = "")
        {
            switch (id)
            {
                case "403":
                    return View("AccessDenied");
                case "404":
                    return View("PageNotFound");
                default:
                    return View();
            }
        }

        //
        // GET: /Common/PageNotFound
        [AllowAnonymous]
        public ActionResult PageNotFound()
        {
            Response.StatusCode = 404;

            return View();
        }

        //
        // GET: /Common/AntiForgery
        [AllowAnonymous]
        public ActionResult AntiForgery()
        {
            return View();
        }

        //
        // GET: /Common/AccessDenied
        [HttpGet]
        [AllowAnonymous]
        public ActionResult AccessDenied()
        {
            return View();
        }

    }
}