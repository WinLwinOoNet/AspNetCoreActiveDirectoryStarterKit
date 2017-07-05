namespace Asp.Core
{
    public class AppSettings
    {
        public Application Application { get; set; }

        public CookieAuthentication CookieAuthentication { get; set; }
    }

    public class Application
    {
        public string Name { get; set; }
        public string Version { get; set; }
    }

    public class CookieAuthentication
    {
        /// <summary>
        /// Session expiration in minutes. Default is 15 minutes
        /// </summary>
        public int ExpireMinutes { get; set; } = 15;

        /// <summary>
        /// Display a message box before session expires. Default is 2 minutes.
        /// </summary>
        public int SessionExpireNotificationMinutes { get; set; } = 2;
    }
}