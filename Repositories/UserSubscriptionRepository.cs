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

        public Task<bool> HasActiveAsync(int userId) => _dao.HasActiveAsync(userId);

        public Task UpsertActivateAsync(int userId, int planId, int months) => _dao.UpsertActivateAsync(userId, planId, months);
    }
}
