using Scradic.Core.Entities;

namespace Scradic.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task RegisterSingleUser(User user);
        Task<User?> GetSingleUser();
        Task UpdateUser(User user);
    }
}