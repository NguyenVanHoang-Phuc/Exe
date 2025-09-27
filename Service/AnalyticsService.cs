using BusinessObject.Models;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IAnalyticsRepository _analyticsRepo;

        public AnalyticsService(IAnalyticsRepository analyticsRepo)
        {
            _analyticsRepo = analyticsRepo;
        }

        public async Task<AnalyticsDto> GetMonthlyAnalyticsAsync(int userId, DateTime start, DateTime end)
        {
            return await _analyticsRepo.GetMonthlyAnalyticsAsync(userId, start, end);
        }
    }
}
