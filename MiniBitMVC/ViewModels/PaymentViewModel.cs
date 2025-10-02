namespace MiniBitMVC.ViewModels
{
    public class PaymentViewModel
    {
        public int UserId { get; set; }
        public decimal Amount { get; set; }

        public string? Currency { get; set; }

        public string? Method { get; set; }

        public string? Status { get; set; }
    }
}
