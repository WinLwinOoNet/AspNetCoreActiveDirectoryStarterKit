using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Core.Domains;

namespace Asp.Web.Common.Security
{
    public interface ISignInManager
    {
        Task SignInAsync(User user, IList<string> roleNames);

        Task SignOutAsync();
    }
}