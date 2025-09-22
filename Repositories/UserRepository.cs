using BusinessObject.Models;
using DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDAO _dao;

        public UserRepository(UserDAO dao)
        {
            _dao = dao;
        }

        public Task<User?> GetUserByIdAsync(int userId) => _dao.GetUserByIdAsync(userId);
        public Task<User?> GetUserByEmailAsync(string email) => _dao.GetUserByEmailAsync(email);

        public Task<bool> UpdateUpdatedAtAsync(int userId) => _dao.UpdateUpdatedAtAsync(userId);

        public Task<User> CreateUserFromGoogleAsync(string email, string name) => _dao.CreateUserFromGoogleAsync(email, name);

        public Task<bool> EmailExistsAsync(string email) => _dao.EmailExistsAsync(email);
        public Task<User> CreateAsync(User user) => _dao.CreateUserAsync(user);

    }
}
