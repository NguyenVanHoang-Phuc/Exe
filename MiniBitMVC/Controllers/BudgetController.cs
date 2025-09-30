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
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetBudgetSummary(int userId)
        {
            var budget = await _budgetService.GetBudgetByUserIdAsync(1);

            var today = DateOnly.FromDateTime(DateTime.Now);
            var startOfMonth = new DateOnly(today.Year, today.Month, 1);

            // Tổng chi tiêu/thanh toán tháng này
            var spent = await _traService.GetMonthlySpendingByUserIdAsync(1);

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

            Console.WriteLine($"UserId={request.UserId}, Amount={request.AmountLimit}, Start={request.StartMonth}, End={request.EndMonth}, Year={request.Year}");

            if (request.StartMonth > request.EndMonth)
                return BadRequest(new { message = "Tháng bắt đầu phải <= tháng kết thúc" });

            var budgets = new List<Budget>();
            for (int m = request.StartMonth; m <= request.EndMonth; m++)
            {
                budgets.Add(new Budget
                {
                    UserId = 1,
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
