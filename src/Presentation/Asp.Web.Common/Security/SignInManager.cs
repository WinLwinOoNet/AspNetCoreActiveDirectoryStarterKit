using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Asp.Core.Domains;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Asp.Web.Common.Security
{
    public class SignInManager : ISignInManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SignInManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        
        public async Task SignInAsync(User user, IList<string> roleNames)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName)
            };

            foreach (string roleName in roleNames)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleName));
            }

            var identity = new ClaimsIdentity(claims, "local", "name", "role");
            var principal = new ClaimsPrincipal(identity);
            
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        public async Task SignOutAsync()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
