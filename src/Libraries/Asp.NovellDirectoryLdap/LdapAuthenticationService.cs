using Asp.Repositories.Authentication;
using Novell.Directory.Ldap;

namespace Asp.NovellDirectoryLdap
{
    /// <summary>
    /// As of today, System.DirectoryServices hasn't been implemented in ASP.NET Core yet, 
    /// so we need to use Novell.Directory.Ldap.NETStandard.
    /// https://github.com/dotnet/corefx/issues/2089
    /// https://github.com/dsbenghe/Novell.Directory.Ldap.NETStandard
    /// </summary>
    public class LdapAuthenticationService : IAuthenticationService
    {
        public bool ValidateUser(string domainName, string username, string password)
        {
            string userDn = $"{username}@{domainName}";
            try
            {
                using (var connection = new LdapConnection {SecureSocketLayer = false})
                {
                    connection.Connect(domainName, LdapConnection.DEFAULT_PORT);
                    connection.Bind(userDn, password);

                    if (connection.Bound)
                        return true;
                }
            }
            catch (LdapException ex)
            {
                // Log exception
            }
            return false;
        }
    }
}