using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Core.Domains;

namespace Asp.Repositories.Messages
{
    public interface IEmailTemplateRepository
    {
        Task<IList<EmailTemplate>> GetAllEmailTemplates();

        Task<EmailTemplate> GetEmailTemplateById(int id);

        Task<EmailTemplate> GetEmailTemplateByName(string name);

        Task<int> InsertEmailTemplate(EmailTemplate template);

        Task UpdateEmailTemplate(EmailTemplate template);
    }
}