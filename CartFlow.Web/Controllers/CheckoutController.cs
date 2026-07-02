using CartFlow.Data; 
using CartFlow.Data.Data;
using CartFlow.Data.Entities;
using CartFlow.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CartFlow.Web.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly AppDbContext _context;

        public CheckoutController(AppDbContext context)
        {
            _context = context;
        }

        // 1. [HttpGet] Index()
        [HttpGet]
        public async Task<IActionResult> Index(int? productId, int? quantity)
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

            // If a single product is passed (Buy Now flow), prepare checkout items from that product
            if (productId.HasValue)
            {
                var prod = await _context.Products
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.Id == productId.Value);

                if (prod == null)
                {
                    return NotFound();
                }

                var qty = (quantity.HasValue && quantity.Value > 0) ? quantity.Value : 1;

                ViewBag.CartItems = new List<CartItemViewModel>
                {
                    new CartItemViewModel
                    {
                        Id = 0,
                        ProductId = prod.Id,
                        ProductName = prod.Name,
                        UnitPrice = prod.UnitPrice,
                        Quantity = qty,
                        ImageUrl = prod.ProductImages?.FirstOrDefault(pi => pi.IsPrimary)?.Image
                                   ?? prod.ProductImages?.FirstOrDefault()?.Image
                                   ?? string.Empty
                    }
                };
            }
            else
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                            .ThenInclude(p => p.ProductImages)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null || !cart.CartItems.Any())
                {
                    return RedirectToAction("Index", "Cart");
                }

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
            }

            // Pre-fill user's FirstName + LastName + Email from Identity
            var viewModel = new CheckoutViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            return View(viewModel);
        }

        // 2. [HttpPost] Index(CheckoutViewModel model)
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

            // Validate model
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

            // Check stock for each item (if insufficient -> ModelState error)
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

            // Create Order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                // OrderStatus = CartFlow.Data.Enums.OrderStatus.Ordered, 
                TotalQuantity = totalQuantity,
                TotalPrice = totalPrice 
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Create OrderItems from cart items
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

                // تحديث المخزون
                item.Product.StockQuantity -= item.Quantity;
            }

            // Clear cart items
            _context.CartItems.RemoveRange(cart.CartItems);

            // SaveChanges
            await _context.SaveChangesAsync();

            // Redirect to Confirmation(order.Id)
            return RedirectToAction(nameof(Confirmation), new { id = order.Id });
        }

        // 3. Confirmation(int id)
        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            var order = await _context.Orders
                .Include(order => order.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var summaryViewModel = new OrderSummaryViewModel
            {
                OrderNumber = order.Id.ToString(), 
                ItemCount = order.TotalQuantity,
                Total = order.TotalPrice
            };

            return View(summaryViewModel);
        }
    }
}

