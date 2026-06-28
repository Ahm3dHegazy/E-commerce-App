using CartFlow.Data.Data;
using CartFlow.Services.Interfaces;
using CartFlow.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartFlow.Web.Controllers;

public class ProductsController(IProductService productService, AppDbContext context) : Controller
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

        return View(viewModel);
    }
}
