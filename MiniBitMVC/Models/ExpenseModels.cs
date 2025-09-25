using BusinessObject.Models;
using System.ComponentModel.DataAnnotations;

namespace MiniBitMVC.Models
{
    public class ExpenseModels
    {

        public class SavingsGoal
        {
            [Range(0, double.MaxValue, ErrorMessage = "Mục tiêu phải lớn hơn hoặc bằng 0")]
            public decimal MonthlyTarget { get; set; }

            [Range(0, double.MaxValue, ErrorMessage = "Giới hạn phải lớn hơn hoặc bằng 0")]
            public decimal DailyLimit { get; set; }
        }
        public class DeleteTransactionRequest
        {
            public int TransactionId { get; set; }
        }

        public class UpdateCategoryRequest
        {
            public int TransactionId { get; set; }
            public int CategoryId { get; set; }
        }

        public class UpdateTransactionTypeRequest
        {
            public int TransactionId { get; set; }
            public string TransactionType { get; set; } // good / bad
        }

        public class DashboardViewModel
        {
            public List<Category> Categories { get; set; } = new List<Category>();
            public List<Transaction> Transactions { get; set; } = new List<Transaction>();
            public SavingsGoal SavingsGoal { get; set; } = new SavingsGoal();
            public bool IsPremium { get; set; }
            public decimal TodayTotal { get; set; }
            public decimal MonthlyTotal { get; set; }
            public decimal MonthlySavings { get; set; }
            public List<string> Notifications { get; set; } = new List<string>();
        }
    }
}
