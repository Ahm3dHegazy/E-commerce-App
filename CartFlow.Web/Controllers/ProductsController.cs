using CartFlow.Data.Data;
using CartFlow.Services.Interfaces;
using CartFlow.Web.Models;
using CartFlow.Services.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartFlow.Web.Controllers;

public class ProductsController(IProductService productService, AppDbContext context, CartFlow.Services.Interfaces.IReviewService reviewService) : Controller
{
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

        // تمرير المراجعات الحقيقية لكروت الصفحة الرئيسية والـ ViewModel سيحسب الـ AverageRating تلقائياً
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

        // جلب المراجعات الـ 9 الحقيقية وضخها في الـ ViewModel ليقوم بحساب المتوسط تلقائياً
        var dtos = (await reviewService.GetReviewsForProductAsync(product.Id)) ?? new List<ReviewDto>();
        viewModel.Reviews = dtos.ToList();

        return View(viewModel);
    }
}