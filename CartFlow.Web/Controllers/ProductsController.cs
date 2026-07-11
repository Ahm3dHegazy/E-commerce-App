using System.Security.Claims;
using CartFlow.Data.Data;
using CartFlow.Services.Interfaces;
using CartFlow.Web.Models;
using CartFlow.Services.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartFlow.Web.Controllers;

public class ProductsController(IProductService productService, AppDbContext context, CartFlow.Services.Interfaces.IReviewService reviewService) : Controller
{
    private const string FavoritesCookieName = "CartFlow_Favorites";

    public async Task<IActionResult> Index(string? searchTerm, int? categoryId)
    {
        var products = await productService.GetAllAsync(searchTerm, categoryId);

        var viewModel = new ProductListViewModel
        {
            Products = products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                UnitPrice = p.UnitPrice,
                StockQuantity = p.StockQuantity,
                CategoryName = p.Category?.Name,
                CategoryId = p.CategoryId,
                ImageUrl = p.ProductImages?.FirstOrDefault()?.Image,
                Initial = string.IsNullOrEmpty(p.Name) ? string.Empty : p.Name[0].ToString().ToUpper()
            }).ToList(),

            SearchTerm = searchTerm,
            SelectedCategoryId = categoryId,
            Categories = await context.Categories.ToListAsync()
        };

        if (viewModel.Products.Any())
        {
            foreach (var p in viewModel.Products)
            {
                p.Reviews = (await reviewService.GetReviewsForProductAsync(p.Id))?.ToList() ?? new List<ReviewDto>();
            }
        }

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return PartialView("_ProductGrid", viewModel);
        }

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await productService.GetByIdAsync(id);
        if (product is null) return NotFound();

        var viewModel = new ProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            UnitPrice = product.UnitPrice,
            StockQuantity = product.StockQuantity,
            CategoryName = product.Category?.Name,
            CategoryId = product.CategoryId,
            ImageUrl = product.ProductImages?.FirstOrDefault()?.Image,
            ImageUrls = product.ProductImages?.Select(pi => pi.Image).ToList() ?? new List<string>(),
            Initial = string.IsNullOrEmpty(product.Name) ? string.Empty : product.Name[0].ToString().ToUpper()
        };

        var dtos = (await reviewService.GetReviewsForProductAsync(product.Id)) ?? new List<ReviewDto>();
        viewModel.Reviews = dtos.ToList();

        return View(viewModel);
    }

    public async Task<IActionResult> Wishlist()
    {
        var favoritesCookie = Request.Cookies[FavoritesCookieName];
        var favoriteIds = new List<int>();

        if (!string.IsNullOrEmpty(favoritesCookie))
        {
            favoriteIds = favoritesCookie.Split(',')
                .Select(id => int.TryParse(id, out var parsedId) ? parsedId : 0)
                .Where(id => id > 0)
                .ToList();
        }

        var allProducts = await productService.GetAllAsync(null, null);
        var favoriteProducts = allProducts.Where(p => favoriteIds.Contains(p.Id)).Select(p => new ProductViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            UnitPrice = p.UnitPrice,
            StockQuantity = p.StockQuantity,
            CategoryName = p.Category?.Name,
            CategoryId = p.CategoryId,
            ImageUrl = p.ProductImages?.FirstOrDefault()?.Image,
            Initial = string.IsNullOrEmpty(p.Name) ? string.Empty : p.Name[0].ToString().ToUpper()
        }).ToList();

        return View(favoriteProducts);
    }

    [HttpPost]
    public IActionResult ToggleFavorite(int productId)
    {
        var favoritesCookie = Request.Cookies[FavoritesCookieName];
        var favoriteIds = new List<string>();

        if (!string.IsNullOrEmpty(favoritesCookie))
        {
            favoriteIds = favoritesCookie.Split(',').ToList();
        }

        bool isAdded;
        if (favoriteIds.Contains(productId.ToString()))
        {
            favoriteIds.Remove(productId.ToString());
            isAdded = false;
        }
        else
        {
            favoriteIds.Add(productId.ToString());
            isAdded = true;
        }

        var options = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(30),
            Path = "/",
            HttpOnly = true
        };

        Response.Cookies.Append(FavoritesCookieName, string.Join(",", favoriteIds), options);

        return Json(new { success = true, isAdded = isAdded });
    }

    [HttpPost]
    public async Task<IActionResult> AddReview(int productId, int rating, string comment)
    {
        if (User.Identity?.IsAuthenticated != true)
            return Challenge();

        if (rating < 1 || rating > 5)
        {
            TempData["Error"] = "Rating must be between 1 and 5.";
            return RedirectToAction("Details", new { id = productId });
        }

        if (string.IsNullOrWhiteSpace(comment))
        {
            TempData["Error"] = "Comment cannot be empty.";
            return RedirectToAction("Details", new { id = productId });
        }

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            return Challenge();

        await reviewService.AddReviewAsync(productId, userId, rating, comment);
        TempData["Success"] = "Review added successfully!";
        return RedirectToAction("Details", new { id = productId });
    }
}