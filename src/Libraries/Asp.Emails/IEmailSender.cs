using System.Collections.Generic;
using System.Threading.Tasks;
using MimeKit;

namespace Asp.Emails
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmailAccount emailAccount, 
            string subject, string body, 
            string fromAddress, 
            IEnumerable<string> toAddresses, 
            IEnumerable<string> bccAddresses = null);

        Task SendEmailAsync(EmailAccount emailAccount, 
            string subject, 
            string body,
            MailboxAddress fromAddress, 
            IEnumerable<MailboxAddress> toAddresses, 
            IEnumerable<MailboxAddress> bccMailAddresses);
    }
}