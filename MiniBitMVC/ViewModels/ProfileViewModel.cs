namespace MiniBitMVC.ViewModels
{
    public class ProfileViewModel
    {
        // Thông tin cơ bản từ User
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsPremium { get; set; } 

        // Các thông tin cá nhân (User)
        public string Phone { get; set; } = string.Empty;
        public string Dob { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;

        // Dữ liệu cá nhân / insights (Tính toán từ Transaction, Goal, Budget)
        public int TxCount { get; set; }  // Số lượng giao dịch
        public int Months { get; set; }   // Số tháng đã sử dụng
        public string Goal { get; set; } = string.Empty;  // Mục tiêu hiện tại
        public string Budget { get; set; } = string.Empty;  // Ngân sách hiện tại
    }
}
