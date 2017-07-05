using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Core.Data;
using Asp.Core.Domains;

namespace Asp.Repositories.Logging
{
    public interface ILogRepository
    {
        Task<IPagedList<Log>> GetLogs(LogPagedDataRequest request);

        Task<IList<string>> GetLevels();
    }
}