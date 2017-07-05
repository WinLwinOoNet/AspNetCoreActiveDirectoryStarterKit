using System;
using System.Linq;
using System.Threading.Tasks;
using Asp.Core.Data;
using Asp.Core.Domains;
using Asp.Data;
using Microsoft.EntityFrameworkCore;

namespace Asp.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbContext _context;

        public UserRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<IPagedList<User>> GetUsersAsync(UserPagedDataRequest request)
        {
            var query = _context.Users
                .Include(x => x.UserRoles)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.LastName))
                query = query.Where(x => x.LastName.StartsWith(request.LastName));

            if (request.RoleId.HasValue)
                query = query.Where(x => x.UserRoles.Any(r => r.RoleId == request.RoleId));

            if (request.IsActive.HasValue)
                query = query.Where(x => x.IsActive == request.IsActive.Value);

            string orderBy = request.SortField.ToString();
            if (QueryHelper.PropertyExists<User>(orderBy))
                query = request.SortOrder == SortOrder.Ascending ? query.OrderByProperty(orderBy) : query.OrderByPropertyDescending(orderBy);
            else
                query = query.OrderBy(x => x.LastLoginDate);

            var result = await PagedList<User>.CreateAsync(query, request.PageIndex, request.PageSize);
            return result;
        }

        public async Task<User> GetUserByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentNullException(nameof(userName));

            var query = _context.Users
                .Where(x => x.UserName == userName);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var query = _context.Users
                .Where(x => x.Id == userId);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> AddUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user.Id;
        }

        public async Task UpdateUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}