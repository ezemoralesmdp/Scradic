using Scradic.Core.Entities;
using Scradic.Core.Interfaces;

namespace Scradic.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository userRepository)
        {
            _repository = userRepository;
        }

        public async Task RegisterSingleUser(User user)
        {
            await _repository.RegisterSingleUser(user);
        }

        public async Task<User?> GetSingleUser()
        {
            return await _repository.GetSingleUser();
        }
    }
}