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
            {
                TempData["Error"] = "Chưa chọn file.";
                return RedirectToAction("Index");
            }

            var transactions = new List<Transaction>();
            string ext = Path.GetExtension(file.FileName).ToLower();

            string NormalizeNumber(string input)
            {
                if (string.IsNullOrWhiteSpace(input)) return "0";
                var clean = new string(input.Where(c => char.IsDigit(c) || c == '.' || c == '-').ToArray());
                return string.IsNullOrWhiteSpace(clean) ? "0" : clean;
            }

            bool TryParseDate(string input, out DateTime date)
            {
                string[] formats = { "dd/MM/yyyy HH:mm", "dd/MM/yyyy" };
                return DateTime.TryParseExact(input.Trim(), formats,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
            }

            // ================= XLSX =================
            if (ext == ".xlsx")
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                using var workbook = new ClosedXML.Excel.XLWorkbook(stream);
                var worksheet = workbook.Worksheets.First();

                bool headerFound = false;
                var colIndex = new Dictionary<string, int>();

                foreach (var row in worksheet.RowsUsed())
                {
                    var rowTexts = row.Cells().Select(c => c.GetString().Replace("\n", " ").Trim()).ToList();
                    var joined = string.Join(" | ", rowTexts);
                    Console.WriteLine("[XLSX ROW] " + joined);

                    // tìm header
                    if (!headerFound)
                    {
                        if (rowTexts.Any(t => t.ToLower().Contains("transaction date")) ||
                            rowTexts.Any(t => t.ToLower().Contains("ngày giao")))
                        {
                            headerFound = true;
                            Console.WriteLine("Header FOUND (XLSX)!");

                            for (int i = 0; i < rowTexts.Count; i++)
                            {
                                var text = rowTexts[i].ToLower();
                                if (text.Contains("transaction date") || text.Contains("ngày giao")) colIndex["date"] = i + 1;
                                else if (text.Contains("debit") || text.Contains("phát sinh n")) colIndex["debit"] = i + 1;
                                else if (text.Contains("credit") || text.Contains("phát sinh có")) colIndex["credit"] = i + 1;
                                else if (text.Contains("details") || text.Contains("nội dung")) colIndex["details"] = i + 1;
                            }

                            Console.WriteLine("Cột map:");
                            foreach (var kv in colIndex)
                                Console.WriteLine($" - {kv.Key} = {kv.Value}");

                            continue;
                        }
                        else continue;
                    }

                    try
                    {
                        if (!colIndex.ContainsKey("date")) continue;

                        var dateStr = row.Cell(colIndex["date"]).GetString().Trim();
                        if (!TryParseDate(dateStr, out var date))
                        {
                            Console.WriteLine($"[SKIP] Không parse được ngày: {dateStr}");
                            continue;
                        }

                        decimal.TryParse(NormalizeNumber(row.Cell(colIndex["debit"]).GetString()), out var debit);
                        decimal.TryParse(NormalizeNumber(row.Cell(colIndex["credit"]).GetString()), out var credit);
                        var details = colIndex.ContainsKey("details") ? row.Cell(colIndex["details"]).GetString() : "";

                        if (details.ToLower().Contains("tổng phát sinh")) continue;

                        var amount = credit - debit;
                        Console.WriteLine($"[XLSX] Date={date:yyyy-MM-dd}, Debit={debit}, Credit={credit}, Amount={amount}, Desc={details}");

                        transactions.Add(new Transaction
                        {
                            UserId = 1,
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

            // ================= CSV =================
            else if (ext == ".csv")
            {
                using var reader = new StreamReader(file.OpenReadStream());
                string? line;
                bool headerFound = false;
                int dateIdx = -1, debitIdx = -1, creditIdx = -1, detailsIdx = -1;

                while ((line = reader.ReadLine()) != null)
                {
                    var cols = line.Split(',');
                    var headerLine = line.Replace("\n", " ").ToLower();

                    if (!headerFound)
                    {
                        if (headerLine.Contains("transaction date") || headerLine.Contains("ngày giao"))
                        {
                            headerFound = true;
                            Console.WriteLine("Header FOUND (CSV)!");

                            for (int i = 0; i < cols.Length; i++)
                            {
                                var text = cols[i].ToLower();
                                if (text.Contains("transaction date") || text.Contains("ngày giao")) dateIdx = i;
                                else if (text.Contains("debit") || text.Contains("phát sinh n")) debitIdx = i;
                                else if (text.Contains("credit") || text.Contains("phát sinh có")) creditIdx = i;
                                else if (text.Contains("details") || text.Contains("nội dung")) detailsIdx = i;
                            }

                            continue;
                        }
                        continue;
                    }

                    try
                    {
                        if (dateIdx == -1) continue;

                        if (!TryParseDate(cols[dateIdx], out var date))
                        {
                            Console.WriteLine($"[SKIP] Không parse được ngày: {cols[dateIdx]}");
                            continue;
                        }

                        decimal.TryParse(NormalizeNumber(cols[debitIdx]), out var debit);
                        decimal.TryParse(NormalizeNumber(cols[creditIdx]), out var credit);
                        var details = detailsIdx >= 0 ? cols[detailsIdx] : "";

                        if (details.ToLower().Contains("tổng phát sinh")) continue;

                        var amount = credit - debit;
                        Console.WriteLine($"[CSV] Date={date:yyyy-MM-dd}, Debit={debit}, Credit={credit}, Amount={amount}, Desc={details}");

                        transactions.Add(new Transaction
                        {
                            UserId = 1,
                            TransactionDate = date,
                            Description = details,
                            Amount = amount
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[CSV] Lỗi dòng: {ex.Message}");
                    }
                }
            }

            Console.WriteLine($"Total transactions parsed: {transactions.Count}");
            foreach (Transaction s in transactions)
            {
                Console.WriteLine($" - {s.TransactionDate:dd/MM/yyyy} | {s.Description} | {s.Amount}");
            }

            await _transactionService.AddListTransactionsAsync(transactions);

            TempData["Success"] = $"Đã import {transactions.Count} giao dịch!";
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> GetTransactions(string type, string date, string week, string month, string from, string to)
        {
            var transactions = await _transactionService.GetTransactionsByUserIdAsync(1);

            if (type == "day" && DateTime.TryParse(date, out var d))
            {
                transactions = transactions.Where(t => t.TransactionDate.Date == d.Date).ToList();
            }
            else if (type == "week" && !string.IsNullOrEmpty(week))
            {
                // week dạng "2025-W38"
                var parts = week.Split("-W");
                if (parts.Length == 2 && int.TryParse(parts[0], out int year) && int.TryParse(parts[1], out int weekNum))
                {
                    // Tính ngày đầu tuần theo chuẩn ISO (thứ 2 là đầu tuần)
                    var jan1 = new DateTime(year, 1, 1);
                    int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

                    var firstThursday = jan1.AddDays(daysOffset);
                    var cal = System.Globalization.CultureInfo.CurrentCulture.Calendar;
                    int firstWeek = cal.GetWeekOfYear(firstThursday,
                        System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

                    int weekNumAdjusted = weekNum;
                    if (firstWeek <= 1) weekNumAdjusted -= 1;

                    var result = firstThursday.AddDays(weekNumAdjusted * 7);
                    var startOfWeek = result.AddDays(-3); // Monday
                    var endOfWeek = startOfWeek.AddDays(7);

                    transactions = transactions
                        .Where(t => t.TransactionDate >= startOfWeek && t.TransactionDate < endOfWeek)
                        .ToList();
                }
            }
            else if (type == "month" && !string.IsNullOrEmpty(month))
            {
                // month dạng "2025-09"
                if (DateTime.TryParse(month + "-01", out var monthDate))
                {
                    var startOfMonth = new DateTime(monthDate.Year, monthDate.Month, 1);
                    var endOfMonth = startOfMonth.AddMonths(1);

                    transactions = transactions
                        .Where(t => t.TransactionDate >= startOfMonth && t.TransactionDate < endOfMonth)
                        .ToList();
                }
            }
            else if (type == "range" && DateTime.TryParse(from, out var f) && DateTime.TryParse(to, out var t))
            {
                transactions = transactions
                    .Where(x => x.TransactionDate.Date >= f.Date && x.TransactionDate.Date <= t.Date)
                    .ToList();
            }

            return Json(transactions);
        }

    }
}