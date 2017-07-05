using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Core.Data;
using Asp.Core.Domains;
using Asp.Data;
using Microsoft.EntityFrameworkCore;

namespace Asp.Repositories.Logging
{
    public class LogRepository : ILogRepository
    {
        private readonly IDbContext _context;

        public LogRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<IPagedList<Log>> GetLogs(LogPagedDataRequest request)
        {
            var query = _context.Logs.AsQueryable();

            if (request.FromDate.HasValue)
                query = query.Where(x => x.Logged >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(x => x.Logged <= request.ToDate.Value);

            if (!string.IsNullOrWhiteSpace(request.Level))
                query = query.Where(x => x.Level == request.Level);

            if (!string.IsNullOrWhiteSpace(request.Message))
                query = query.Where(x => x.Message.Contains(request.Message));

            if (!string.IsNullOrWhiteSpace(request.Logger))
                query = query.Where(x => x.Logger.Contains(request.Logger));

            if (!string.IsNullOrWhiteSpace(request.Callsite))
                query = query.Where(x => x.Callsite.Contains(request.Callsite));

            if (!string.IsNullOrWhiteSpace(request.Exception))
                query = query.Where(x => x.Exception != null && x.Exception.Contains(request.Exception));

            string orderBy = request.SortField.ToString();
            if (QueryHelper.PropertyExists<Log>(orderBy))
                query = request.SortOrder == SortOrder.Ascending ? query.OrderByProperty(orderBy) : query.OrderByPropertyDescending(orderBy);
            else
                query = query.OrderByDescending(x => x.Logged);

            return await PagedList<Log>.CreateAsync(query, request.PageIndex, request.PageSize);
        }

        public async Task<IList<string>> GetLevels()
        {
            var query = _context.Logs.AsQueryable()
                .Select(x => x.Level)
                .Distinct();

            return await query.ToListAsync();
        }
    }
}