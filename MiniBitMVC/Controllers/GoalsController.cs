using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Service;
using static MiniBitMVC.Models.DTOModel;

namespace MiniBitMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoalsController : ControllerBase
    {
        private readonly IGoalService _goalService;
        private readonly IBudgetService _budgetService;
        private readonly ITransactionService _traService;

        public GoalsController(IGoalService goalService, IBudgetService budgetService, ITransactionService traService)
        {
            _goalService = goalService;
            _budgetService = budgetService;
            _traService = traService;
        }

        // Tạo goal
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] GoalCreateDto goalDto)
        {
            if (goalDto.EndDate.HasValue && goalDto.StartDate.HasValue && goalDto.EndDate < goalDto.StartDate)
                return BadRequest("EndDate phải sau StartDate");

            var goal = new Goal
            {
                UserId = goalDto.UserId,
                Name = goalDto.Name,
                TargetAmount = goalDto.TargetAmount,
                CurrentAmount = goalDto.CurrentAmount,
                StartDate = goalDto.StartDate ?? DateOnly.FromDateTime(DateTime.Today),
                EndDate = goalDto.EndDate,
                CreatedAt = DateTime.UtcNow,
                Status = "open"
            };

            var createdGoal = await _goalService.CreateGoalAsync(goal);

            var result = new GoalDto
            {
                GoalId = createdGoal.GoalId,
                UserId = createdGoal.UserId,
                Name = createdGoal.Name,
                TargetAmount = createdGoal.TargetAmount,
                CurrentAmount = createdGoal.CurrentAmount,
                StartDate = createdGoal.StartDate,
                EndDate = createdGoal.EndDate,
                Status = createdGoal.Status,
                Budgets = createdGoal.Budgets.Select(b => new BudgetDto
                {
                    BudgetId = b.BudgetId,
                    Month = b.Month,
                    Year = b.Year,
                    AmountLimit = b.AmountLimit
                }).ToList()
            };

            return Ok(result);
        }

        // Lấy tất cả goals của user
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetGoalsByUser(int userId)
        {
            var goals = await _goalService.GetGoalsByUserAsync(userId);

            var result = goals.Select(g => new GoalDto
            {
                GoalId = g.GoalId,
                UserId = g.UserId,
                Name = g.Name,
                TargetAmount = g.TargetAmount,
                CurrentAmount = g.CurrentAmount,
                StartDate = g.StartDate,
                EndDate = g.EndDate,
                Status = g.Status,
                Budgets = g.Budgets.Select(b => new BudgetDto
                {
                    BudgetId = b.BudgetId,
                    Month = b.Month,
                    Year = b.Year,
                    AmountLimit = b.AmountLimit
                }).ToList()
            });

            return Ok(result);
        }

        // Xóa goal
        [HttpDelete("{goalId}")]
        public async Task<IActionResult> Delete(int goalId)
        {
            await _goalService.DeleteGoalAsync(goalId);
            return NoContent();
        }

        // Lấy goal active + tiến độ
        [HttpGet("active/{userId}")]
        public async Task<IActionResult> GetActiveGoal(int userId)
        {
            var result = await _goalService.GetActiveGoalWithProgressAsync(userId);
            if (result == null) return Ok(null); // JS hide goalSummary nếu null

            var (goal, savedAmount, progressPercent) = result.Value;

            var dto = new GoalProgressDto
            {
                GoalId = goal.GoalId,
                GoalName = goal.Name,
                TargetAmount = goal.TargetAmount,
                StartDate = goal.StartDate,
                EndDate = goal.EndDate ?? DateOnly.FromDateTime(DateTime.Today),
                SavedAmount = savedAmount,
                ProgressPercent = progressPercent
            };

            Console.WriteLine($"GoalId: {dto.GoalId}, GoalName: {dto.GoalName}, TargetAmount: {dto.TargetAmount}, StartDate: {dto.StartDate}, Deadline: {dto.EndDate}, SavedAmount: {dto.SavedAmount}, ProgressPercent: {dto.ProgressPercent}");

            return Ok(dto);
        }

        [HttpGet("budget/{userId}")]
        public async Task<IActionResult> GetBudget(int userId)
        {
            var budget = await _budgetService.GetBudgetByUserIdAsync(userId);

            var today = DateOnly.FromDateTime(DateTime.Now);
            var startOfMonth = new DateOnly(today.Year, today.Month, 1);

            var spent = await _traService.GetTotalSavedByUserInRangeAsync(userId, startOfMonth, today);

            return Ok(new BudgetSummaryDto
            {
                BudgetMonth = budget?.AmountLimit ?? 0,
                SpentAmount = spent,
                ThresholdPercent = 80
            });
        }

    }
}
