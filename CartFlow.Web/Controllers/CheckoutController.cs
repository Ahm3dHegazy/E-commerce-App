using CartFlow.Services.Interfaces;
using CartFlow.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace CartFlow.Web.Controllers {
    public class CheckoutController : Controller {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(ICheckoutService checkoutService) {
            _checkoutService = checkoutService;
        }

        public IActionResult Index() {
            return View(new CheckoutViewModel());
        }

        public IActionResult Confirmation() {
            var data = _checkoutService.GetConfirmation();
            return View(new OrderSummaryViewModel {
                OrderNumber = data.OrderNumber,
                ItemCount = data.ItemCount,
                Total = data.Total
            });
        }
    }
}
