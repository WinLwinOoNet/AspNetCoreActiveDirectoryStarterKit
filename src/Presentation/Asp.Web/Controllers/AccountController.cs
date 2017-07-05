using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Core;
using Asp.Repositories.Authentication;
using Asp.Repositories.Domains;
using Asp.Repositories.Roles;
using Asp.Repositories.Users;
using Asp.Web.Areas.Administration.Controllers;
using Asp.Web.Common.Models.AccountViewModels;
using Asp.Web.Common.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
namespace Asp.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IDateTime _dateTime;
        private readonly IDomainRepository _domainRepository;
        private readonly ILogger<AccountController> _logger;
        private readonly IRoleRepository _roleRepository;
        private readonly ISignInManager _signInManager;
        private readonly IUserRepository _userRepository;

        public AccountController(
            IAuthenticationService authenticationService, 
            IDateTime dateTime, 
            IDomainRepository domainRepository,
            ILogger<AccountController> logger,
            IRoleRepository roleRepository,
            ISignInManager signInManager, 
            IUserRepository userRepository)
        {
            _authenticationService = authenticationService;
            _dateTime = dateTime;
            _domainRepository = domainRepository;
            _logger = logger;
            _roleRepository = roleRepository;
            _signInManager = signInManager;
            _userRepository = userRepository;
        }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await _signInManager.SignOutAsync();

            ViewData["ReturnUrl"] = returnUrl;
            var model = new LoginViewModel { AvailableDomains = await GetDomains() };
            return View("Login", model);
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                bool result = _authenticationService.ValidateUser(model.Domain, model.UserName, model.Password);
                if (result)
                {
                    var user = await _userRepository.GetUserByUserNameAsync(model.UserName);
                    if (user != null && user.IsActive)
                    {
                        var roleNames = (await _roleRepository.GetRolesForUser(user.Id)).Select(r => r.Name).ToList();
                        await _signInManager.SignInAsync(user, roleNames);
                        
                        user.LastLoginDate = _dateTime.Now;
                        await _userRepository.UpdateUserAsync(user);
                        
                        _logger.LogInformation($"Login Successful: {user.UserName}.");

                        if (!string.IsNullOrEmpty(returnUrl) && !string.Equals(returnUrl, "/") && Url.IsLocalUrl(returnUrl))
                            return RedirectToLocal(returnUrl);
                        
                        if (roleNames.Contains(Constants.RoleNames.Administrator))
                            return RedirectToAction(nameof(DashboardController.Index), "Dashboard", new {area = Constants.Areas.Administration});

                        return RedirectToAction(nameof(HomeController.Index), "Home");
                    }
                    _logger.LogError($"Authorization Fail: {model.UserName}");
                    ModelState.AddModelError("", Constants.Messages.AccessDenied);
                }
                else
                {
                    _logger.LogError($"Login Fail: {model.UserName} - Incorrect username or password. ");
                    ModelState.AddModelError("", "Incorrect username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            model.AvailableDomains = await GetDomains();
            return View("Login", model);
        }

        //
        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation($"Logout Successful: {User.Identity.Name}");
            return RedirectToAction(nameof(AccountController.Login));
        }

        private async Task<IList<SelectListItem>> GetDomains()
        {
            return (await _domainRepository.GetAllDomainsAsync())
                .Select(d => new SelectListItem { Text = d.Name, Value = d.Name })
                .ToList();
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}