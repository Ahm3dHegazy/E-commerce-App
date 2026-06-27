using CartFlow.Services.Interfaces;
using CartFlow.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace CartFlow.Web.Controllers {
    public class CartController : Controller {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService) {
            _cartService = cartService;
        }

        public async Task<IActionResult> Index() {
            var items = await _cartService.GetCartItemsAsync();

            var model = new CartViewModel {
                Items = items.Select(ci => new CartItemViewModel {
                    ProductName = ci.Product?.Name ?? "",
                    CategoryName = ci.Product?.Category?.Name ?? "",
                    UnitPrice = ci.UnitPrice,
                    Quantity = ci.Quantity
                }).ToList()
            };

            return View(model);
        }
    }
}
