using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IAnalyticsRepository
    {
        Task<AnalyticsDto> GetMonthlyAnalyticsAsync(int userId, DateTime start, DateTime end);
    }
}
