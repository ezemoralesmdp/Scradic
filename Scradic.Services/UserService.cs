using Scradic.Core.Entities;
using Scradic.Core.Interfaces;
using Scradic.Utils.Resources;

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

        public async Task UpdateUser(User user)
        {
            var confirm = "";
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Username: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(user.Username);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Email: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(user.Email);

            do
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{Globals.Warning} ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"{Messages.UpdateUser_Ask} ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(Globals.Y_N);
                Console.Write($"{Globals.Answer} ");
                confirm = Console.ReadLine().ToLower();

            } while (confirm != "y" && confirm != "n");
            
            if(confirm == "y")
            {
                do
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"{Messages.EnterNewUsername} ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    user.Username = Console.ReadLine();

                } while (string.IsNullOrEmpty(user.Username));

                do
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"{Messages.EnterNewEmail} ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    user.Email = Console.ReadLine();

                } while (string.IsNullOrEmpty(user.Email));
            }

            await _repository.UpdateUser(user);

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{Globals.Warning} ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Messages.UserUpdateSuccessfully);
        }
    }
}