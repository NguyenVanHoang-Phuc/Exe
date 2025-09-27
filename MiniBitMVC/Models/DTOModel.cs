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

        public class GoalDto
        {
            public int GoalId { get; set; }
            public int UserId { get; set; }
            public string Name { get; set; } = string.Empty;
            public decimal TargetAmount { get; set; }
            public decimal CurrentAmount { get; set; }
            public DateOnly? StartDate { get; set; }   // mới
            public DateOnly? EndDate { get; set; }     // mới
            public string? Status { get; set; }
            public List<BudgetDto> Budgets { get; set; } = new();
        }

        // DTO cho input (create/update)
        public class GoalCreateDto
        {
            public int UserId { get; set; }
            public string Name { get; set; } = string.Empty;
            public decimal TargetAmount { get; set; }
            public decimal CurrentAmount { get; set; } = 0;

            // Thêm StartDate và EndDate
            public DateOnly? StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
            public DateOnly? EndDate { get; set; }                 // Deadline
        }

        public class GoalProgressDto
        {
            public int GoalId { get; set; }
            public string GoalName { get; set; } = string.Empty;
            public decimal TargetAmount { get; set; }
            public DateOnly? StartDate { get; set; }   // thêm
            public DateOnly? EndDate { get; set; }     // thêm
            public decimal SavedAmount { get; set; }
            public decimal ProgressPercent { get; set; }
        }

        public class BudgetSummaryDto
        {
            public decimal BudgetMonth { get; set; }
            public decimal SpentAmount { get; set; }
            public decimal ThresholdPercent { get; set; } = 80; // mặc định 80%
        }
    }
}
