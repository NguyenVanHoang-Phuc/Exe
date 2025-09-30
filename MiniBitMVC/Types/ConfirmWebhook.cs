namespace MiniBitMVC.Types
{
    public record ConfirmWebhook(
        string webhook_url,
        string PaymentId,
        int UserId,
        string Status,
        decimal Amount,
        string Currency,
        DateTime PaymentDate
    );
}
