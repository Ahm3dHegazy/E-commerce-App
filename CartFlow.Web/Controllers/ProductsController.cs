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

        // Populate reviews for each product (in-memory via IReviewService). This provides
        // average rating and review counts for the index product cards without changing
        // the details page behaviour.
        if (viewModel.Products.Any())
        {
            var reviewTasks = viewModel.Products.Select(p => reviewService.GetReviewsForProductAsync(p.Id)).ToList();
            var reviewResults = await Task.WhenAll(reviewTasks);

            for (int i = 0; i < viewModel.Products.Count; i++)
            {
                viewModel.Products[i].Reviews = reviewResults[i].ToList();
            }
        }

        // If this is an AJAX request return only the products grid partial so we can update the list dynamically.
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
            Initial = string.IsNullOrEmpty(product.Name) ? string.Empty : product.Name[0].ToString().ToUpper()
        };

        // Attach reviews from IReviewService (currently in-memory). Map service DTOs to Web view models.
        var dtos = (await reviewService.GetReviewsForProductAsync(product.Id)) ?? new List<CartFlow.Services.Models.ReviewDto>();
        // ProductViewModel.Reviews uses the services-level ReviewDto to avoid circular dependencies — assign directly.
        viewModel.Reviews = dtos.ToList();

        return View(viewModel);
    }


}
