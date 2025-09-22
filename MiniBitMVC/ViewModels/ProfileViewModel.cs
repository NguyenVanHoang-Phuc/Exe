namespace MiniBitMVC.ViewModels
{
    public class ProfileViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsPremium { get; set; }

        // Thông tin gói Premium
        public string PremiumPlanName { get; set; } = string.Empty; // Tên gói Premium
        public decimal PremiumPrice { get; set; } // Giá của gói
        public int PremiumDurationMonths { get; set; } // Thời gian gói (số tháng)
        public string? PremiumFeatures { get; set; } // Tính năng của gói Premium

        // Các thông tin khác liên quan đến cá nhân và tài chính
        public string Phone { get; set; } = string.Empty;
        public string Dob { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;

        // Tài chính
        public string Currency { get; set; } = "VND"; // Mặc định là VND
        public string Income { get; set; } = string.Empty;
        public string SpendMethod { get; set; } = string.Empty;

        // Dữ liệu cá nhân / insights
        public int TxCount { get; set; }
        public int Months { get; set; }
        public string Goal { get; set; } = string.Empty;
        public string Budget { get; set; } = string.Empty;
    }
}
