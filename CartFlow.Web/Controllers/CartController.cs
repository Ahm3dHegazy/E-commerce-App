using CartFlow.Data.Data;
using CartFlow.Data.Entities;
using CartFlow.Services.Interfaces;
using CartFlow.Web.Models;
using CartFlow.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CartFlow.Web.Controllers {
	public class CartController : Controller {
		private readonly AppDbContext _context;
		private readonly ICartService _cartService;

		public CartController(AppDbContext context, ICartService cartService) {
			_context = context;
			_cartService = cartService;
		}


		public async Task<IActionResult> Index() {
			var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var viewModel = new CartViewModel();

			// If user is not authenticated, try loading anonymous cart from session
			if (string.IsNullOrEmpty(userIdString)) {
				// Try to load anonymous cart from session
				var sessionItems = HttpContext.Session.GetObject<List<CartItemViewModel>>("Cart");
				if (sessionItems != null) {
					viewModel.Items = sessionItems;
				}

				return View(viewModel);
			}

			int userId = int.Parse(userIdString);

			// Load cart for authenticated user
			var cart = await _context.Carts
				.Include(c => c.CartItems)
				.ThenInclude(ci => ci.Product)
				.ThenInclude(p => p.Category)
				.Include(c => c.CartItems)
				.ThenInclude(ci => ci.Product)
				.ThenInclude(p => p.ProductImages)
				.FirstOrDefaultAsync(c => c.UserId == userId);

			if (cart != null) {
				viewModel.Items = cart.CartItems.Select(ci => new CartItemViewModel {
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return NotFound();
            }

            int cartCount;

            if (string.IsNullOrEmpty(userIdString))
            {
                // Anonymous user: store cart in session
                var sessionCart = HttpContext.Session.GetObject<List<CartItemViewModel>>("Cart") ??
                                  new List<CartItemViewModel>();
                var existing = sessionCart.FirstOrDefault(ci => ci.ProductId == productId);
                if (existing != null)
                {
                    existing.Quantity += quantity;
                    existing.UnitPrice = product.UnitPrice;
                }
                else
                {
                    sessionCart.Add(new CartItemViewModel
                    {
                        ProductId = productId,
                        ProductName = product.Name,
                        CategoryName = product.Category?.Name ?? "No Category",
                        UnitPrice = product.UnitPrice,
                        Quantity = quantity,
                        ImageUrl = product.ProductImages?.FirstOrDefault(pi => pi.IsPrimary)?.Image
                                   ?? product.ProductImages?.FirstOrDefault()?.Image
                                   ?? string.Empty
                    });
                }

                HttpContext.Session.SetObject("Cart", sessionCart);
                cartCount = sessionCart.Sum(ci => ci.Quantity);
            }
            else
            {
                int userId = int.Parse(userIdString);

                // Find or create cart for authenticated user
                var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = userId
                    };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync(); // save to generate id
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

                cartCount = await _context.CartItems
                    .Where(ci => ci.CartId == cart.Id)
                    .SumAsync(ci => ci.Quantity);
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, cartCount });
            }

            // Return user to referring page when possible
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
		public async Task<IActionResult> RemoveFromCart(int cartItemId) {
			var cartItem = await _context.CartItems.FindAsync(cartItemId);
			if (cartItem != null) {
				_context.CartItems.Remove(cartItem);
				await _context.SaveChangesAsync();
			}

			return RedirectToAction(nameof(Index));
		}

		// 4. [HttpPost] UpdateQuantity: تحديث الكمية
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity) {
			var cartItem = await _context.CartItems.FindAsync(cartItemId);
			if (cartItem != null) {
				if (quantity <= 0) {
					_context.CartItems.Remove(cartItem);
				} else {
					cartItem.Quantity = quantity;
				}

				await _context.SaveChangesAsync();
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ClearCart() {
			await _cartService.ClearCartAsync();
			return RedirectToAction(nameof(Index));
		}
	}
}