using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class AnalyticsDto
    {
        public decimal TotalMonth { get; set; }      // Tổng chi tiêu
        public decimal TotalIncome { get; set; }     // Tổng thu
        public decimal TotalBalance { get; set; }    // Thu - chi
        public decimal AvgPerDay { get; set; }
        public CategoryDto? TopCategory { get; set; }
        public decimal BadRate { get; set; }
        public int BadCount { get; set; }
        public List<CategoryAmountDto> Categories { get; set; } = new();
        public List<DailyAmountDto> Daily { get; set; } = new();
        public List<TrendDto> Trend { get; set; } = new();
        public List<CategoryAmountDto> Top5 { get; set; } = new();
        public string? SmartTip { get; set; }
    }

    public class CategoryDto { public string Name { get; set; } = ""; public decimal Amount { get; set; } }
    public class CategoryAmountDto { public string Name { get; set; } = ""; public decimal Amount { get; set; } public string? Color { get; set; } }
    public class DailyAmountDto { public int Day { get; set; } public decimal Amount { get; set; } }
    public class TrendDto { public string Month { get; set; } = ""; public decimal Amount { get; set; } }

}
