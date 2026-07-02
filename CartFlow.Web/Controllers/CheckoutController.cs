using CartFlow.Data;
using CartFlow.Data.Data;
using CartFlow.Data.Entities;
using CartFlow.Data.Enums;
using CartFlow.Services.Interfaces;
using CartFlow.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace CartFlow.Web.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IStripePaymentService _paymentService;
        private readonly IConfiguration _configuration;

        public CheckoutController(AppDbContext context, IStripePaymentService paymentService, IConfiguration configuration)
        {
            _context = context;
            _paymentService = paymentService;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdString);
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var viewModel = new CheckoutViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            ViewBag.CartItems = cart.CartItems.Select(ci => new CartItemViewModel
            {
                Id = ci.Id,
                ProductId = ci.ProductId,
                ProductName = ci.Product.Name,
                UnitPrice = ci.UnitPrice,
                Quantity = ci.Quantity,
                ImageUrl = ci.Product.ProductImages?.FirstOrDefault(pi => pi.IsPrimary)?.Image
                    ?? ci.Product.ProductImages?.FirstOrDefault()?.Image
                    ?? string.Empty
            }).ToList();

            ViewBag.StripePublishableKey = _configuration["StripeKeys:PublishableKey"];

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CheckoutViewModel model)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdString);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            foreach (var item in cart.CartItems)
            {
                if (item.Product.StockQuantity < item.Quantity)
                {
                    ModelState.AddModelError("", $"Sorry, the product '{item.Product.Name}' does not have enough stock. Available: {item.Product.StockQuantity}");
                    return View(model);
                }
            }

            decimal subtotal = cart.CartItems.Sum(item => item.Quantity * item.UnitPrice);
            decimal shipping = 50.00m;
            decimal totalPrice = subtotal + shipping;
            int totalQuantity = cart.CartItems.Sum(item => item.Quantity);

            PaymentMethod paymentMethod = model.PaymentMethod == "CreditCard" ? PaymentMethod.Credit : PaymentMethod.Cash;

            string? stripePaymentIntentId = null;

            if (paymentMethod == PaymentMethod.Credit)
            {
                if (string.IsNullOrEmpty(model.PaymentMethodId))
                {
                    ModelState.AddModelError("", "Credit card information is required.");
                    return View(model);
                }

                var paymentResult = await _paymentService.CreatePaymentIntentAsync(totalPrice, "egp", model.PaymentMethodId);

                if (!paymentResult.Success)
                {
                    ModelState.AddModelError("", $"Payment failed: {paymentResult.ErrorMessage}");
                    return View(model);
                }

                stripePaymentIntentId = paymentResult.PaymentIntentId;
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                OrderStatus = Status.Ordered,
                TotalQuantity = totalQuantity,
                TotalPrice = totalPrice,
                PaymentMethod = paymentMethod,
                StripePaymentIntentId = stripePaymentIntentId,
                AddressLine1 = model.AddressLine1,
                City = model.City,
                State = model.State,
                PostalCode = model.PostalCode,
                Country = model.Country
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cart.CartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.UnitPrice
                };
                _context.OrderItems.Add(orderItem);

                item.Product.StockQuantity -= item.Quantity;
            }

            _context.CartItems.RemoveRange(cart.CartItems);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Confirmation), new { id = order.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var summaryViewModel = new OrderSummaryViewModel
            {
                OrderNumber = order.Id.ToString(),
                ItemCount = order.TotalQuantity,
                Total = order.TotalPrice,
                PaymentMethod = order.PaymentMethod == PaymentMethod.Credit ? "Credit Card" : "Cash on Delivery"
            };

            return View(summaryViewModel);
        }
    }
}
