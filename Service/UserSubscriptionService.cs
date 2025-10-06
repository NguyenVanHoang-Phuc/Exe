using BusinessObject.Models;
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

        public async Task<UserSubscription> GetActiveSubscriptionAsync(int userId) => await _userSubscriptionRepository.GetActiveSubscriptionAsync(userId);
        public Task<bool> HasActiveAsync(int userId) => _userSubscriptionRepository.HasActiveAsync(userId);

        public async Task UpsertActivateAsync(int userId, int months, decimal amount) => await _userSubscriptionRepository.UpsertActivateAsync(userId, months, amount);
    }
}
