using news_api.Data.Repositories;
using news_api.DTO;
using news_api.Models;
using System.Security.Cryptography;
using System.Text;

namespace news_api.Services
{
    public interface IUserService
    {
        UserModel RegisterUser(User user);
        UserModel AuthenticateUser(string username, string password);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public UserModel RegisterUser(User user)
        {
            var userModel = new UserModel
            {
                Username = user.Username,
                Email = user.Email,
                PasswordHash = user.Password,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _userRepository.AddUser(userModel);
            return userModel;
        }

        public UserModel AuthenticateUser(string username, string password)
        {
            var user = _userRepository.GetUserByUsername(username);

            if (user == null || !VerifyPasswordHash(password, user.PasswordHash))
            {
                return null;
            }

            return user;
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            using (var hmac = new HMACSHA512())
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
