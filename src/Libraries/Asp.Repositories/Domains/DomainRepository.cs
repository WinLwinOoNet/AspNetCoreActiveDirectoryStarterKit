using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Core.Domains;
using Asp.Data;
using Microsoft.EntityFrameworkCore;

namespace Asp.Repositories.Domains
{
    public class DomainRepository : IDomainRepository
    {
        private readonly IDbContext _context;

        public DomainRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<IList<Domain>> GetAllDomainsAsync()
        {
            var query = _context.Domains
                .AsQueryable();

            return await query.ToListAsync();
        }

        public async Task<Domain> GetDomainByIdAsync(int userId)
        {
            var query = _context.Domains
                .Where(x => x.Id == userId);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> AddDomainAsync(Domain user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _context.Domains.Add(user);
            await _context.SaveChangesAsync();

            return user.Id;
        }

        public async Task UpdateDomainAsync(Domain user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _context.Domains.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}