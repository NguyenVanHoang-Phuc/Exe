using BusinessObject.Models;
using DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CategoryDAO _categoryDAO;

        public CategoryRepository(CategoryDAO categoryDAO)
        {
            _categoryDAO = categoryDAO;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _categoryDAO.GetAllAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _categoryDAO.GetByIdAsync(id);
        }

        public async Task<List<Category>> GetByUserIdAsync(int userId)
        {
            return await _categoryDAO.GetByUserIdAsync(userId);
        }

        public async Task AddAsync(Category category)
        {
            await _categoryDAO.AddAsync(category);
        }

        public async Task UpdateAsync(Category category)
        {
            await _categoryDAO.UpdateAsync(category);
        }

        public async Task DeleteAsync(int id)
        {
            await _categoryDAO.DeleteAsync(id);
        }
    }
}
