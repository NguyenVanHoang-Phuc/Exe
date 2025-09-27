using BusinessObject.Models;
using GroqNet;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace Service
{
    public class AIService : IAIService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly ITransactionRepository _transRepo; // 👈 thêm repository

        public AIService(IConfiguration config, ITransactionRepository transRepo)   // 👈 inject
        {
            _apiKey = config["Groq:ApiKey"]
                      ?? throw new ArgumentNullException("Groq:ApiKey chưa được cấu hình trong appsettings.json");

            _transRepo = transRepo ?? throw new ArgumentNullException(nameof(transRepo)); // 👈 gán repository

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<string> GetFinancialAdviceAsync(int userId, string prompt)
        {
            // 1. Lấy giao dịch thực tế của user
            var transactions = await _transRepo.GetTransactionsByUserIdAsync(userId);

            // 2. Tính toán tổng chi tiêu tháng này và tháng trước
            var now = DateTime.Now;
            var thisMonthTotal = transactions
                .Where(t => t.TransactionDate.Month == now.Month && t.TransactionDate.Year == now.Year)
                .Sum(t => t.Amount);
            var lastMonthTotal = transactions
                .Where(t => t.TransactionDate.Month == now.AddMonths(-1).Month
                         && t.TransactionDate.Year == now.AddMonths(-1).Year)
                .Sum(t => t.Amount);

            // 3. Tạo prompt cho AI, kèm dữ liệu thực tế
            var fullPrompt = $@"
                            User có chi tiêu tháng trước: {lastMonthTotal}đ, chi tiêu tháng này: {thisMonthTotal}đ.
                            {prompt}
                            Hãy đưa ra lời khuyên chi tiết, dễ hiểu và cụ thể dựa trên số liệu này.
                            ";

            // 4. Gọi API Groq với fullPrompt
            var requestBody = new
            {
                model = "llama-3.1-8b-instant",
                messages = new[]
                {
                    new { role = "system", content = "Bạn là trợ lý tài chính, hãy trả lời ngắn gọn và dễ hiểu." },
                    new { role = "user", content = fullPrompt }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://api.groq.com/openai/v1/chat/completions", content);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonResponse);
            var reply = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return reply ?? "❌ Bot không có phản hồi.";
        }
    }
}
