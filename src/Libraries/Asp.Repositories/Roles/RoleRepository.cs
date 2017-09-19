using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Core.Domains;
using Asp.Data;
using Microsoft.EntityFrameworkCore;

namespace Asp.Repositories.Roles
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IDbContext _context;

        public RoleRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<IList<Role>> GetAllRolesAsync()
        {
            var query = _context.Roles;

            return await query.ToListAsync();
        }

        public async Task<IList<Role>> GetRolesForUserAsync(int userId)
        {
            var query = _context.Roles
                .Where(r => r.UserRoles.Any(u => u.UserId == userId))
                .Select(r => r)
                .OrderBy(r => r.Name);

            return await query.ToListAsync();
        }

        public async Task<IList<UserRole>> GetUserRolesForUserAsync(int userId)
        {
            var query = _context.UserRoles
                .Where(r => r.UserId == userId)
                .Select(r => r);

            return await query.ToListAsync();
        }
    }
}