using CartFlow.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace CartFlow.Services.Services {
	public class StripePaymentService : IStripePaymentService {
		private readonly IConfiguration _configuration;

		public StripePaymentService(IConfiguration configuration) {
			_configuration = configuration;
			StripeConfiguration.ApiKey = configuration["StripeKeys:SecretKey"];
		}

		public async Task<PaymentIntentResult> CreatePaymentIntentAsync(decimal amount, string currency,
			string paymentMethodId) {
			try {
			var options = new PaymentIntentCreateOptions {
				Amount = (long)(amount * 100),
				Currency = currency,
				PaymentMethod = paymentMethodId,
				ConfirmationMethod = "automatic",
				Confirm = true,
				AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions {
					Enabled = true,
					AllowRedirects = "never"
				},
			};

				var service = new PaymentIntentService();
				var intent = await service.CreateAsync(options);

				if (intent.Status == "succeeded" || intent.Status == "requires_capture") {
					return new PaymentIntentResult {
						Success = true,
						PaymentIntentId = intent.Id,
						ClientSecret = intent.ClientSecret
					};
				}

				return new PaymentIntentResult {
					Success = false,
					PaymentIntentId = intent.Id,
					ErrorMessage = intent.Status
				};
			}
			catch (StripeException ex) {
				return new PaymentIntentResult {
					Success = false,
					ErrorMessage = ex.Message
				};
			}
		}
	}
}