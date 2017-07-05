using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Asp.Core;
using Asp.Core.Domains;
using Asp.Emails;
using Asp.Repositories.Settings;

namespace Asp.Repositories.Messages
{
    public class MessageService : IMessageService
    {
        private const string EmailFromAddress = "email.from.address";
        private const string EmailToAddresses = "email.to.addresses";
        private const string EmailSmtpHost = "email.smtp.host";
        private const string WebsiteUrl = "website.url";

        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly ISettingRepository _settingRepository;
        private readonly IEmailSender _emailSender;

        public MessageService(
            IEmailTemplateRepository emailTemplateRepository,
            ISettingRepository settingRepository,
            IEmailSender emailSender)
        {
            _emailTemplateRepository = emailTemplateRepository;
            _settingRepository = settingRepository;
            _emailSender = emailSender;
        }

        private string ReplaceMessageTemplateTokens(User user, string websiteUrl, string template)
        {
            var tokens = new Dictionary<string, string>()
            {
                {"[[[User_FirstName]]]", WebUtility.HtmlEncode(user.FirstName)},
                {"[[[User_LastName]]]", WebUtility.HtmlEncode(user.LastName)},
                {"[[[User_FullName]]]", WebUtility.HtmlEncode(user.FirstName + " " + user.LastName)},
                {"[[[User_EditLink]]]", $"<a href=\"{websiteUrl}/users/edit/{user.Id}\">here</a>"},
                {"[[[User_EditUrl]]]", $"{websiteUrl}/users/edit/{user.Id}"},
                {"[[[Website_Link]]]", $"<a href=\"{websiteUrl}\">Sample Application</a>"},
            };

            // Replaces tokens in the template with the values.
            foreach (string token in tokens.Keys)
                template = template.Replace(token, tokens[token]);

            return template;
        }

        public async Task SendAddNewUserNotification(User user)
        {
            string fromAddress = _settingRepository.GetSettingByKey<string>(EmailFromAddress, ""),
                toAddresses = _settingRepository.GetSettingByKey<string>(EmailToAddresses, ""),
                smtpHost = _settingRepository.GetSettingByKey<string>(EmailSmtpHost, ""),
                websiteUrl = _settingRepository.GetSettingByKey<string>(WebsiteUrl, "");

            if (string.IsNullOrWhiteSpace(fromAddress) || string.IsNullOrWhiteSpace(toAddresses) ||
                string.IsNullOrWhiteSpace(smtpHost) || string.IsNullOrWhiteSpace(websiteUrl))
                throw new Exception("Email configuration hasn't been set up yet.");

            var toAddressCollection = toAddresses.Split(new[] {",", ";"}, StringSplitOptions.RemoveEmptyEntries).ToList();
            var emailAccount = new EmailAccount {Host = smtpHost};

            var template = await _emailTemplateRepository.GetEmailTemplateByName(
                Constants.EmailTemplates.AddNewUserNotification);
            string subject = ReplaceMessageTemplateTokens(user, websiteUrl, template.Subject);
            string body = ReplaceMessageTemplateTokens(user, websiteUrl, template.Body);

            await _emailSender.SendEmailAsync(emailAccount, subject, body, fromAddress, toAddressCollection);
        }
    }
}