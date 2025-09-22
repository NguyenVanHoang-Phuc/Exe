using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class UserSubscriptionService : IUserSubscriptionService
    {
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;

        public UserSubscriptionService(IUserSubscriptionRepository userSubscriptionRepository)
        {
            _userSubscriptionRepository = userSubscriptionRepository;
        }

        public Task<bool> HasActiveAsync(int userId) => _userSubscriptionRepository.HasActiveAsync(userId);

        public Task UpsertActivateAsync(int userId, int planId, int months) => _userSubscriptionRepository.UpsertActivateAsync(userId, planId, months);
    }
}
