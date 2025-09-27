using BusinessObject.Models;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _budgetRepository;

        public BudgetService(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }
        public async Task<Budget?> GetBudgetByUserIdAsync(int userId)
        {
            return await _budgetRepository.GetBudgetByUserIdAsync(userId);
        }
    }
}
