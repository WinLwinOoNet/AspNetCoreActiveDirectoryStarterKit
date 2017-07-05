using System.Threading.Tasks;
using Asp.Core.Domains;

namespace Asp.Repositories.Messages
{
    public interface IMessageService
    {
        Task SendAddNewUserNotification(User user);
    }
}