namespace Asp.Core.Data
{
    public class UserPagedDataRequest : PagedDataRequest
    {
        public int? RoleId { get; set; }
        public string LastName { get; set; }
        public bool? IsActive { get; set; }

        public UserSortField SortField { get; set; }
        public SortOrder SortOrder { get; set; }

        public UserPagedDataRequest()
        {
            SortOrder = SortOrder.Ascending;
            SortField = UserSortField.LastLoginDate;
        }
    }
    public enum UserSortField
    {
        UserName,
        FirstName,
        LastName,
        Initials,
        IsActive,
        LastLoginDate,
        CreatedOn,
        CreatedBy,
        ModifiedOn,
        ModifiedBy
    }
}