using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject
{
    public class UserDAO
    {
        private readonly FinanceAppDbContext _context;

        public UserDAO(FinanceAppDbContext context)
        {
            _context = context;
        }

        public Task<User?> GetUserByIdAsync(int userId) => _context.Users.Include(u => u.Role).AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId);


        // Lấy user theo email, bao gồm thông tin role
        public Task<User?> GetUserByEmailAsync(string email) => _context.Users.Include(u => u.Role).AsNoTracking().FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        // Cập nhật trường UpdatedAt của user
        public async Task<bool> UpdateUpdatedAtAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<User> CreateUserFromGoogleAsync(string email, string name)
        {
            var user = new User
            {
                RoleId = 1,
                Email = email,
                Name = name,
                PasswordHash = "", // No password for Google-authenticated users
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public Task<bool> EmailExistsAsync(string email) => _context.Users.AsNoTracking().AnyAsync(u => u.Email == email);

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
