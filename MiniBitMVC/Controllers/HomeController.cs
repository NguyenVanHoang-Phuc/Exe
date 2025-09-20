using BusinessObject.Models;
using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniBitMVC.Models;
using Service;
using System.Diagnostics;
using System.Globalization;
using static MiniBitMVC.Models.ExpenseModels;

namespace MiniBitMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static SavingsGoal _savingsGoal = new SavingsGoal();
        private readonly ITransactionService _transactionService;
        private static bool _isPremium = false;

        public HomeController(ILogger<HomeController> logger, ITransactionService transactionService)
        {
            _logger = logger;
            _transactionService = transactionService;
        }

        // GET: Dashboard
        public async Task<IActionResult> Index()
        {
            var transactions = await _transactionService.GetTransactionsByUserIdAsync(1);

            var model = new DashboardViewModel
            {
                Transactions = transactions,
                SavingsGoal = _savingsGoal,
                IsPremium = _isPremium
            };

            // Calculate statistics
            var today = DateTime.Today;
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            model.TodayTotal = transactions
                .Where(t => t.TransactionDate.Date == today)
                .Sum(t => t.Amount);

            model.MonthlyTotal = transactions
                .Where(t => t.TransactionDate.Month == currentMonth && t.TransactionDate.Year == currentYear)
                .Sum(t => t.Amount);

            model.MonthlySavings = _savingsGoal.MonthlyTarget - model.MonthlyTotal;

            // Generate notifications
            model.Notifications = GenerateNotifications(transactions, model);

            return View(model);
        }

        // POST: Add Transaction
        [HttpPost]
        public async Task<IActionResult> AddTransaction([FromBody] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                transaction.TransactionDate = DateTime.Now;
                await _transactionService.AddTransactionAsync(transaction); // Lưu vào DB

                return Json(new { success = true, message = "Giao dịch đã được thêm thành công!" });
            }

            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }

        // POST: Save Savings Goal
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

        // POST: Upgrade to Premium
        [HttpPost]
        public IActionResult UpgradeToPremium()
        {
            _isPremium = true;
            return Json(new { success = true, message = "Chào mừng bạn đến với MiniBit Premium!" });
        }

        // Generate notifications
        private List<string> GenerateNotifications(List<Transaction> transactions, DashboardViewModel model)
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

            // Yesterday notification
            var yesterday = DateTime.Today.AddDays(-1);
            var yesterdayTotal = transactions
                .Where(t => t.TransactionDate.Date == yesterday)
                .Sum(t => t.Amount);

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

        public IActionResult FAQ()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> ImportTransactions(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Chưa chọn file.");

            var transactions = new List<Transaction>();
            string ext = Path.GetExtension(file.FileName).ToLower();

            string NormalizeNumber(string input)
            {
                if (string.IsNullOrWhiteSpace(input)) return "0";
                var clean = new string(input.Where(c => char.IsDigit(c) || c == '.' || c == '-').ToArray());
                return string.IsNullOrWhiteSpace(clean) ? "0" : clean;
            }

            if (ext == ".xlsx")
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheets.First();

                bool headerFound = false;
                foreach (var row in worksheet.RowsUsed())
                {
                    if (!headerFound)
                    {
                        var headerText = string.Join(" | ",
                            row.Cells().Select(c => c.GetString().Replace("\n", " ").Trim().ToLower()));
                        Console.WriteLine("[XLSX HEADER] " + headerText);

                        if (headerText.Contains("transaction date"))
                        {
                            headerFound = true;
                            Console.WriteLine("Header FOUND (XLSX)!");
                        }
                        continue;
                    }

                    try
                    {
                        var dateStr = row.Cell(3).GetString().Trim();
                        if (!DateTime.TryParse(dateStr, out var date)) continue;

                        decimal.TryParse(NormalizeNumber(row.Cell(5).GetString()), out var debit);
                        decimal.TryParse(NormalizeNumber(row.Cell(6).GetString()), out var credit);
                        var details = row.Cell(7).GetString();

                        var amount = credit - debit;
                        Console.WriteLine($"[XLSX] Date={date:yyyy-MM-dd}, Debit={debit}, Credit={credit}, Amount={amount}, Desc={details}");

                        if (amount == 0) continue;

                        transactions.Add(new Transaction
                        {
                            TransactionDate = date,
                            Description = details,
                            Amount = amount
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[XLSX] Lỗi dòng: {ex.Message}");
                    }
                }
            }
            else if (ext == ".csv")
            {
                using var reader = new StreamReader(file.OpenReadStream());
                string? line;
                bool headerFound = false;

                while ((line = reader.ReadLine()) != null)
                {
                    if (!headerFound)
                    {
                        var headerLine = line.Replace("\n", " ").ToLower();
                        Console.WriteLine("[CSV HEADER] " + headerLine);

                        if (headerLine.Contains("transaction date"))
                        {
                            headerFound = true;
                            Console.WriteLine("Header FOUND (CSV)!");
                        }
                        continue;
                    }

                    var cols = line.Split(',');
                    if (cols.Length < 7) continue;

                    if (!DateTime.TryParse(cols[2], out var date)) continue;

                    decimal.TryParse(NormalizeNumber(cols[4]), out var debit);
                    decimal.TryParse(NormalizeNumber(cols[5]), out var credit);
                    var details = cols[6];

                    var amount = credit - debit;
                    Console.WriteLine($"[CSV] Date={date:yyyy-MM-dd}, Debit={debit}, Credit={credit}, Amount={amount}, Desc={details}");

                    if (amount == 0) continue;

                    transactions.Add(new Transaction
                    {
                        TransactionDate = date,
                        Description = details,
                        Amount = amount
                    });
                }
            }

            return Ok(new { count = transactions.Count });
        }








        [HttpGet]
        public async Task<IActionResult> GetTransactionsByDate(DateTime date)
        {
            var transactions = await _transactionService.GetTransactionsByUserIdAsync(1);
            var filtered = transactions
                .Where(t => t.TransactionDate.Date == date.Date)
                .Select(t => new
                {
                    t.TransactionDate,
                    t.Description,
                    t.Category,
                    t.TransactionType,
                    t.Amount
                }).ToList();

            return Json(filtered);
        }

    }
}