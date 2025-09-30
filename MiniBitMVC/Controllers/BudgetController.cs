using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using static MiniBitMVC.Models.DTOModel;

namespace MiniBitMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetController : ControllerBase
    {
        private readonly IBudgetService _budgetService;
        private readonly ITransactionService _traService;

        public BudgetController(IBudgetService budgetService, ITransactionService traService)
        {
            _budgetService = budgetService;
            _traService = traService;
        }

        // Lấy thông tin ngân sách tháng này của user
        [HttpGet]
        public async Task<IActionResult> GetBudgetSummary()
        {
            int? sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (sessionUserId == null)
                return Unauthorized(new { message = "Chưa đăng nhập" });
            int userId = sessionUserId.Value;

            var budget = await _budgetService.GetBudgetByUserIdAsync(userId);

            var today = DateOnly.FromDateTime(DateTime.Now);
            var startOfMonth = new DateOnly(today.Year, today.Month, 1);

            // Tổng chi tiêu/thanh toán tháng này
            var spent = await _traService.GetMonthlySpendingByUserIdAsync(userId);

            return Ok(new BudgetSummaryDto
            {
                BudgetMonth = budget?.AmountLimit ?? 0,
                SpentAmount = spent,
                ThresholdPercent = 80
            });
        }

        [HttpPost("add-range")]
        public async Task<IActionResult> AddBudgets([FromBody] AddBudgetsRequest request)
        {
            int? sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (sessionUserId == null)
                return Unauthorized(new { message = "Chưa đăng nhập" });
            int userId = sessionUserId.Value;

            Console.WriteLine($"UserId={userId}, Amount={request.AmountLimit}, Start={request.StartMonth}, End={request.EndMonth}, Year={request.Year}");

            if (request.StartMonth > request.EndMonth)
                return BadRequest(new { message = "Tháng bắt đầu phải <= tháng kết thúc" });

            var budgets = new List<Budget>();
            for (int m = request.StartMonth; m <= request.EndMonth; m++)
            {
                budgets.Add(new Budget
                {
                    UserId = userId,
                    Month = m,
                    Year = request.Year,
                    AmountLimit = request.AmountLimit,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _budgetService.AddRangeAsync(budgets);

            return Ok(new { message = "Thêm ngân sách thành công", count = budgets.Count });
        }


    }
}
