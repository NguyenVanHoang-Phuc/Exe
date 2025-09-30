namespace MiniBitMVC.Models
{
    public class DTOModel
    {
        public class BudgetDto
        {
            public int BudgetId { get; set; }
            public int Month { get; set; }
            public int Year { get; set; }
            public decimal AmountLimit { get; set; }
        }

        // Goal DTO (bỏ Budgets)
        public class GoalDto
        {
            public int GoalId { get; set; }
            public int UserId { get; set; }
            public string Name { get; set; } = string.Empty;
            public decimal TargetAmount { get; set; }
            public decimal CurrentAmount { get; set; }
            public DateOnly? StartDate { get; set; }
            public DateOnly? EndDate { get; set; }
            public string? Status { get; set; }
        }

        // DTO cho input (create/update goal)
        public class GoalCreateDto
        {
            public int UserId { get; set; }
            public string Name { get; set; } = string.Empty;
            public decimal TargetAmount { get; set; }
            public decimal CurrentAmount { get; set; } = 0;
            public DateOnly? StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
            public DateOnly? EndDate { get; set; }
        }

        // Goal progress
        public class GoalProgressDto
        {
            public int GoalId { get; set; }
            public string GoalName { get; set; } = string.Empty;
            public decimal TargetAmount { get; set; }
            public DateOnly? StartDate { get; set; }
            public DateOnly? EndDate { get; set; }
            public decimal SavedAmount { get; set; }
            public decimal ProgressPercent { get; set; }
        }

        // Budget summary (giữ riêng)
        public class BudgetSummaryDto
        {
            public decimal BudgetMonth { get; set; }
            public decimal SpentAmount { get; set; }
            public decimal ThresholdPercent { get; set; } = 80; // mặc định 80%
        }

        public class AddBudgetsRequest
        {
            public int UserId { get; set; }
            public decimal AmountLimit { get; set; }
            public int StartMonth { get; set; }
            public int EndMonth { get; set; }
            public int Year { get; set; }
        }
    }
}
