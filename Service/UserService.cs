using BusinessObject.Models;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<User?> GetUserByIdAsync(int userId) => _userRepository.GetUserByIdAsync(userId);
        public Task<User?> GetUserByEmailAsync(string email) => _userRepository.GetUserByEmailAsync(email);

        public Task<bool> UpdateUpdatedAtAsync(int userId) => _userRepository.UpdateUpdatedAtAsync(userId);

        public Task<User> CreateUserFromGoogleAsync(string email, string name) => _userRepository.CreateUserFromGoogleAsync(email, name);
        public Task<bool> EmailExistsAsync(string email) => _userRepository.EmailExistsAsync(email);
        public Task<User> CreateAsync(User user) => _userRepository.CreateAsync(user);
    }
}
