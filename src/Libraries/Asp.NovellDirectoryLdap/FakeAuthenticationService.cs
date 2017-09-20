using Asp.Repositories.Authentication;

namespace Asp.NovellDirectoryLdap
{
    /// <summary>
    /// Fake AuthenticationService for Integration Tests and UI Tests.
    /// </summary>
    public class FakeAuthenticationService : IAuthenticationService
    {
        public bool ValidateUser(string domain, string userName, string password)
        {
            return domain == "domain2.com" && userName == "johndoe" && password == "Sw@*V9Rk";
        }
    }
}
