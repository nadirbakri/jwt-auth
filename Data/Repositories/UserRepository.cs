using news_api.Models;
using System.Collections.Generic;

namespace news_api.Data.Repositories
{
    public interface IUserRepository
    {
        UserModel GetUserByUsername(string username);
        void AddUser(UserModel user);
        // Tambahkan metode lain sesuai kebutuhan
    }

    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public UserModel GetUserByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }

        public void AddUser(UserModel user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
    }
}

