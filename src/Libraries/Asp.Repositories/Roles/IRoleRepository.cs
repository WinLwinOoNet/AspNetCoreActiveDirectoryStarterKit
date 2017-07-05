using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Core.Domains;

namespace Asp.Repositories.Roles
{
    public interface IRoleRepository
    {
        Task<IList<Role>> GetAllRoles();

        Task<IList<Role>> GetRolesForUser(int userId);

        Task<IList<UserRole>> GetUserRolesForUser(int userId);
    }
}