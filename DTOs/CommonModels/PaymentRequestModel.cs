namespace DTOs.CommonModels
{
    public class PaymentRequestModel
    {
        public int UserId { get; set; }  // ID của người dùng
        public decimal Amount { get; set; }  // Số tiền thanh toán
        public string Currency { get; set; }  // Tiền tệ (VND, USD, v.v.)
        public string PaymentMethod { get; set; }  // Phương thức thanh toán (credit card, paypal, v.v.)
    }
}
