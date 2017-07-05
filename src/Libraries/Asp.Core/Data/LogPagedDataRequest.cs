using System;

namespace Asp.Core.Data
{
    public class LogPagedDataRequest : PagedDataRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Logger { get; set; }
        public string Callsite { get; set; }
        public string Exception { get; set; }
        public LogSortField SortField { get; set; }
        public SortOrder SortOrder { get; set; }

        public LogPagedDataRequest()
        {
            SortOrder = SortOrder.Descending;
            SortField = LogSortField.Logged;
        }
    }

    public enum LogSortField
    {
        Id,
        Application,
        Logged,
        Level,
        Message,
        Logger,
        Callsite,
        Exception,
    }
}
