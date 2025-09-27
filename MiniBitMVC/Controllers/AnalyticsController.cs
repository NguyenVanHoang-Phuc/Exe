using Microsoft.AspNetCore.Mvc;
using Service;

namespace MiniBitMVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("monthly/{userId}")]
        public async Task<IActionResult> GetMonthlyAnalytics(int userId)
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var endOfMonth = now;

            var analytics = await _analyticsService.GetMonthlyAnalyticsAsync(userId, startOfMonth, endOfMonth);
            return Ok(analytics);
        }
    }
}
