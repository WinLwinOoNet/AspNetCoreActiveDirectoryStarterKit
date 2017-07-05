using System;

namespace Asp.Web.Common.Models.LogViewModels
{
    public class LogViewModel
    {
        public int Id { get; set; }
        public DateTime Logged { get; set; }
        public string Level { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string Identity { get; set; }
        public string Referrer { get; set; }
        public string UserAgent { get; set; }
        public string Url { get; set; }
        public string Message { get; set; }
        public string Logger { get; set; }
        public string Callsite { get; set; }
        public string Exception { get; set; }
    }
}
