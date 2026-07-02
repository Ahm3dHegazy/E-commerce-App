namespace CartFlow.Web.Models
{
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
