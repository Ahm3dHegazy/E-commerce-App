using CartFlow.Data;
using CartFlow.Data.Data;
using CartFlow.Data.Entities;
using CartFlow.Services.Interfaces;
using CartFlow.Services.Services;
using CartFlow.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CartFlow.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ICartService _cartService;

        public CartController(AppDbContext context,ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        // 1. Index() : عرض السلة وتفاصيلها بناءً على الـ Cart و الـ ViewModels
        public async Task<IActionResult> Index()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdString);

            // جلب السلة بناءً على الـ UserId الخاص بـ Cart Flow Entities
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Category)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            var viewModel = new CartViewModel();

            if (cart != null)
            {
                viewModel.Items = cart.CartItems.Select(ci => new CartItemViewModel
                {
                    Id = ci.Id,
                    ProductName = ci.Product.Name,
                    CategoryName = ci.Product.Category?.Name ?? "No Category",
                    UnitPrice = ci.UnitPrice,
                    Quantity = ci.Quantity,
                    ImageUrl = ci.Product.ProductImages?.FirstOrDefault(pi => pi.IsPrimary)?.Image
                        ?? ci.Product.ProductImages?.FirstOrDefault()?.Image
                        ?? string.Empty
                }).ToList();
            }

            return View(viewModel);
        }

        // 2. [HttpPost] AddToCart: إضافة منتج للسلة أو تحديثه لـ User الحالي
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdString);

            // البحث عن السلة الخاصة بالمستخدم أو إنشاؤها
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync(); // لحفظ السلة وتوليد الـ Id لها
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
                cartItem.UnitPrice = product.UnitPrice;
            }
            else
            {
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = product.UnitPrice
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            // جلب الرابط الأصلي الذي ضغط منه المستخدم وإعادته إليه مجدداً
            string? returnUrl = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }

        // 3. [HttpPost] RemoveFromCart: حذف عنصر من السلة
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // 4. [HttpPost] UpdateQuantity: تحديث الكمية
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                if (quantity <= 0)
                {
                    _context.CartItems.Remove(cartItem);
                }
                else
                {
                    cartItem.Quantity = quantity;
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCart()
        {
            await _cartService.ClearCartAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}