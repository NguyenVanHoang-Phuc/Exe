using BusinessObject.Models;
using DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class UserSubscriptionRepository : IUserSubscriptionRepository
    {
        private readonly UserSubscriptionDAO _dao;

        public UserSubscriptionRepository(UserSubscriptionDAO dao)
        {
            _dao = dao;
        }
        public async Task<UserSubscription> GetActiveSubscriptionAsync(int userId) => await _dao.GetActiveSubscriptionAsync(userId);

        public Task<bool> HasActiveAsync(int userId) => _dao.HasActiveAsync(userId);

        public async Task UpsertActivateAsync(int userId, int months, decimal amount) => await _dao.UpsertActivateAsync(userId, months, amount);
    }
}
