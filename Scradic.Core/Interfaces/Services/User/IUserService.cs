using Scradic.Core.Entities;

namespace Scradic.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task RegisterSingleUser(User user);
        Task<User?> GetSingleUser();
        Task UpdateUser(User user);
    }
}