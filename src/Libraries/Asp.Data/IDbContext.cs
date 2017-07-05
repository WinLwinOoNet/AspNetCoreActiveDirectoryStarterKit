using System.Threading;
using System.Threading.Tasks;
using Asp.Core.Domains;
using Microsoft.EntityFrameworkCore;

namespace Asp.Data
{
    public interface IDbContext
    {
        DbSet<Domain> Domains { get; set; }
        DbSet<EmailTemplate> EmailTemplates { get; set; }
        DbSet<Log> Logs { get; set; }
        DbSet<Role> Roles { get; set; }
        DbSet<Setting> Settings { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<UserRole> UserRoles { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
