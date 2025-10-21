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

        // ✅ Lấy tất cả ngân sách theo user
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetBudgetsByUser(int userId)
        {
            var budgets = await _budgetService.GetAllByUserIdAsync(userId);
            if (budgets == null || !budgets.Any())
                return Ok(new List<object>());

            // Ghép thêm thông tin đã chi
            var result = new List<object>();
            foreach (var b in budgets)
            {
                var spent = await _traService.GetMonthlySpendingByUserIdAsync(userId);
                result.Add(new
                {
                    b.BudgetId,
                    b.AmountLimit,
                    b.Month,
                    b.Year,
                    SpentAmount = spent
                });
            }

            return Ok(result);
        }

        // ✅ Lấy ngân sách tháng hiện tại
        [HttpGet]
        public async Task<IActionResult> GetBudgetSummary()
        {
            int? sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (sessionUserId == null)
                return Unauthorized(new { message = "Chưa đăng nhập" });

            int userId = sessionUserId.Value;
            var budget = await _budgetService.GetBudgetByUserIdAsync(userId);
            var spent = await _traService.GetMonthlySpendingByUserIdAsync(userId);

            return Ok(new BudgetSummaryDto
            {
                BudgetMonth = budget?.AmountLimit ?? 0,
                SpentAmount = spent,
                ThresholdPercent = 80
            });
        }

        // ✅ Tạo ngân sách (thêm 1 hoặc nhiều tháng)
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] AddBudgetsRequest request)
        {
            int? sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (sessionUserId == null)
                return Unauthorized(new { message = "Chưa đăng nhập" });

            int userId = sessionUserId.Value;

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

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBudgetDto update)
        {
            int? sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (sessionUserId == null)
                return Unauthorized(new { message = "Chưa đăng nhập" });

            int userId = sessionUserId.Value;

            var budget = await _budgetService.GetByIdAsync(id);
            if (budget == null)
                return NotFound(new { message = "Không tìm thấy ngân sách" });

            // ✅ Kiểm tra ngân sách này có thuộc về user hiện tại không
            if (budget.UserId != userId)
                return Forbid(); // hoặc return BadRequest(new { message = "Không có quyền chỉnh sửa ngân sách này" });

            // ✅ Cập nhật dữ liệu
            budget.AmountLimit = update.AmountLimit;
            budget.Month = update.Month;
            budget.Year = update.Year;

            await _budgetService.UpdateAsync(budget);
            return Ok(new { message = "Cập nhật ngân sách thành công" });
        }

        // ✅ Xóa ngân sách
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var budget = await _budgetService.GetByIdAsync(id);
            if (budget == null)
                return NotFound(new { message = "Không tìm thấy ngân sách để xóa" });

            await _budgetService.DeleteAsync(budget);
            return Ok(new { message = "Xóa ngân sách thành công" });
        }
    }
}
