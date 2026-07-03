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
            var reviewTasks = viewModel.Products.Select(p => reviewService.GetReviewsForProductAsync(p.Id)).ToList();
            var reviewResults = await Task.WhenAll(reviewTasks);

            for (int i = 0; i < viewModel.Products.Count; i++)
            {
                viewModel.Products[i].Reviews = reviewResults[i]?.ToList() ?? new List<ReviewDto>();
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

    public async Task<IActionResult> Favorites()
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
}