namespace CartFlow.Services.Interfaces {
	public interface IStripePaymentService {
		Task<PaymentIntentResult> CreatePaymentIntentAsync(decimal amount, string currency, string paymentMethodId);
	}

	public class PaymentIntentResult {
		public bool Success { get; set; }
		public string? PaymentIntentId { get; set; }
		public string? ErrorMessage { get; set; }
		public string? ClientSecret { get; set; }
	}
}