namespace CartFlow.Web.Models
{
    public class OrderSummaryViewModel
    {
        public string OrderNumber { get; set; } = string.Empty;

        public int ItemCount { get; set; }

        public decimal Total { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;
    }
}
