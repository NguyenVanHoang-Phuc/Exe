using Microsoft.AspNetCore.Mvc;
using MiniBitMVC.Models;
using System.Diagnostics;
using static MiniBitMVC.Models.ExpenseModels;

namespace MiniBitMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static List<Expense> _expenses = new List<Expense>();
        private static SavingsGoal _savingsGoal = new SavingsGoal();
        private static bool _isPremium = false;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var model = new DashboardViewModel
            {
                Expenses = _expenses,
                SavingsGoal = _savingsGoal,
                IsPremium = _isPremium
            };

            // Calculate statistics
            var today = DateTime.Today;
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            model.TodayTotal = _expenses
                .Where(e => e.Date.Date == today)
                .Sum(e => e.Amount);

            model.MonthlyTotal = _expenses
                .Where(e => e.Date.Month == currentMonth && e.Date.Year == currentYear)
                .Sum(e => e.Amount);

            model.MonthlySavings = _savingsGoal.MonthlyTarget - model.MonthlyTotal;

            // Generate notifications
            model.Notifications = GenerateNotifications(model);

            return View(model);
        }

        [HttpPost]
        public IActionResult AddExpense([FromBody] Expense expense)
        {
            if (ModelState.IsValid)
            {
                expense.Id = _expenses.Count + 1;
                expense.Date = DateTime.Now;
                _expenses.Add(expense);

                return Json(new { success = true, message = "Chi tiêu đã được thêm thành công!" });
            }

            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }

        [HttpPost]
        public IActionResult SaveSavingsGoal([FromBody] SavingsGoal goal)
        {
            if (ModelState.IsValid)
            {
                _savingsGoal = goal;
                return Json(new { success = true, message = "Mục tiêu đã được lưu thành công!" });
            }

            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }

        [HttpPost]
        public IActionResult UpgradeToPremium()
        {
            _isPremium = true;
            return Json(new { success = true, message = "Chào mừng bạn đến với MiniBit Premium!" });
        }

        private List<string> GenerateNotifications(DashboardViewModel model)
        {
            var notifications = new List<string>();

            // Daily limit check
            if (_savingsGoal.DailyLimit > 0 && model.TodayTotal > _savingsGoal.DailyLimit)
            {
                notifications.Add($"⚠️ Bạn đã vượt mức chi tiêu hàng ngày! ({model.TodayTotal:N0}đ / {_savingsGoal.DailyLimit:N0}đ)");
            }

            // Monthly warning
            if (_savingsGoal.MonthlyTarget > 0 && model.MonthlyTotal > _savingsGoal.MonthlyTarget * 0.8m)
            {
                notifications.Add("🚨 Chi tiêu tháng này đã đạt 80% mục tiêu tiết kiệm!");
            }

            // Yesterday notification (simulated)
            var yesterday = DateTime.Today.AddDays(-1);
            var yesterdayTotal = _expenses
                .Where(e => e.Date.Date == yesterday)
                .Sum(e => e.Amount);

            if (yesterdayTotal > 0)
            {
                notifications.Add($"📊 Hôm qua bạn đã chi tiêu {yesterdayTotal:N0}đ");
            }

            return notifications;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
