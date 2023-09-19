using Scradic.Core.Entities;

namespace Scradic.Core.Interfaces
{
    public interface IUserService
    {
        Task RegisterSingleUser(User user);
        Task<User?> GetSingleUser();
    }
}