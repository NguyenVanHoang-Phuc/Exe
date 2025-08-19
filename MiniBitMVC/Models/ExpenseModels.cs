using System.ComponentModel.DataAnnotations;

namespace MiniBitMVC.Models
{
    public class ExpenseModels
    {
        public class Expense
        {
            public int Id { get; set; }

            [Required]
            [Range(1, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
            public decimal Amount { get; set; }

            [Required]
            public string Category { get; set; }

            public string Description { get; set; }

            [Required]
            public ExpenseType Type { get; set; }

            public DateTime Date { get; set; } = DateTime.Now;
        }

        public enum ExpenseType
        {
            Good = 1,
            Bad = 2
        }

        public class SavingsGoal
        {
            [Range(0, double.MaxValue, ErrorMessage = "Mục tiêu phải lớn hơn hoặc bằng 0")]
            public decimal MonthlyTarget { get; set; }

            [Range(0, double.MaxValue, ErrorMessage = "Giới hạn phải lớn hơn hoặc bằng 0")]
            public decimal DailyLimit { get; set; }
        }

        public class DashboardViewModel
        {
            public List<Expense> Expenses { get; set; } = new List<Expense>();
            public SavingsGoal SavingsGoal { get; set; } = new SavingsGoal();
            public bool IsPremium { get; set; }
            public decimal TodayTotal { get; set; }
            public decimal MonthlyTotal { get; set; }
            public decimal MonthlySavings { get; set; }
            public List<string> Notifications { get; set; } = new List<string>();
        }
    }
}
