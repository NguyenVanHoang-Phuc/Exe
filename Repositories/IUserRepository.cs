using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(int userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> UpdateUpdatedAtAsync(int userId);
        Task<User> CreateUserFromGoogleAsync(string email, string name);
        Task<bool> EmailExistsAsync(string email);
        Task<User> CreateAsync(User user);
    }
}
