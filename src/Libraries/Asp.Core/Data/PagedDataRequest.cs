namespace Asp.Core.Data
{
    public class PagedDataRequest
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public PagedDataRequest()
        {
            PageIndex = 0;
            PageSize = 2147483647;
        }
    }
}