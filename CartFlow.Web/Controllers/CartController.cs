using CartFlow.Data; 
using CartFlow.Data.Data;
using CartFlow.Data.Entities;  
using CartFlow.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CartFlow.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
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
                .FirstOrDefaultAsync(c => c.UserId == userId);

            var viewModel = new CartViewModel();

            if (cart != null)
            {
                viewModel.Items = cart.CartItems.Select(ci => new CartItemViewModel
                {
                    // ملحوظة: تأكد من إضافة Id في الـ CartItemViewModel إذا كنت تستخدمه في الـ View لأزرار الحذف والتعديل
                    ProductName = ci.Product.Name,
                    CategoryName = ci.Product.Category?.Name ?? "No Category",
                    UnitPrice = ci.UnitPrice,
                    Quantity = ci.Quantity
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
                    // تم إزالة السجلات غير الموجودة بالـ Entity مثل CreatedAt ليتطابق مع كودك
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
    }
}