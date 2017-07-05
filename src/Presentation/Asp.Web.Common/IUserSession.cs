namespace Asp.Web.Common
{
    public interface IUserSession
    {
        bool IsAuthenticated { get; }
        int Id { get; }
        string FirstName { get; }
        string LastName { get; }
        string UserName { get; }
        string FullName { get; }
        string EMRSystem { get; }
        string ViewType { get; }
        bool IsInRole(string roleName);
    }
}