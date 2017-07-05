using System.Threading.Tasks;
using Asp.Core.Data;
using Asp.Core.Domains;

namespace Asp.Repositories.Users
{
    public interface IUserRepository
    {
        Task<IPagedList<User>> GetUsersAsync(UserPagedDataRequest request);

        Task<User> GetUserByUserNameAsync(string userName);

        Task<User> GetUserByIdAsync(int userId);

        Task<int> AddUserAsync(User user);

        Task UpdateUserAsync(User user);
    }
}