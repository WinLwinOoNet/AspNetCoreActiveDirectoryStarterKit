using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace Asp.Emails
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(
            EmailAccount emailAccount,
            string subject,
            string body,
            string fromAddress,
            IEnumerable<string> toAddresses,
            IEnumerable<string> bccAddresses)
        {
            var fromMailboxAddress = new MailboxAddress(fromAddress);

            var toMailboxAddresses = new List<MailboxAddress>();
            foreach (var toAddress in toAddresses.Where(x => !string.IsNullOrWhiteSpace(x)))
                toMailboxAddresses.Add(new MailboxAddress(toAddress));

            var bccMailboxAddresses = new List<MailboxAddress>();
            if (bccAddresses != null)
            {
                foreach (var address in bccAddresses.Where(x => !string.IsNullOrWhiteSpace(x)))
                    bccMailboxAddresses.Add(new MailboxAddress(address.Trim()));
            }

            await SendEmailAsync(emailAccount,
                subject,
                body,
                fromMailboxAddress,
                toMailboxAddresses,
                bccMailboxAddresses);
        }

        public async Task SendEmailAsync(
            EmailAccount emailAccount,
            string subject,
            string body,
            MailboxAddress fromAddress,
            IEnumerable<MailboxAddress> toAddresses,
            IEnumerable<MailboxAddress> bccMailboxAddresses)
        {
            var message = new MimeMessage();

            message.From.Add(fromAddress);

            foreach (var toAddress in toAddresses)
                message.To.Add(toAddress);

            foreach (var address in bccMailboxAddresses)
                message.Bcc.Add(address);

            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Html) {Text = body};

            using (var client = new SmtpClient())
            {
                client.LocalDomain = emailAccount.LocalDomain;
                await client.ConnectAsync(emailAccount.Host, emailAccount.Port, SecureSocketOptions.None).ConfigureAwait(false);
                await client.SendAsync(message).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }
    }
}