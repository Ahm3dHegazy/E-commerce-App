using CartFlow.Data.Data;
using CartFlow.Data.Entities;
using CartFlow.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CartFlow.Services.Services;

public class ProductService(AppDbContext context) : IProductService
{
    public async Task<List<Product>> GetFeaturedAsync(int count)
    {
        var products = await context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Include(p => p.Reviews)
            .ToListAsync();

        return products
            .GroupBy(p => p.CategoryId)
            .SelectMany(g => g.OrderBy(_ => Random.Shared.Next()).Take(1))
            .OrderBy(_ => Random.Shared.Next())
            .Take(count)
            .ToList();
    }

    public async Task<List<Product>> GetAllAsync(string? searchTerm = null, int? categoryId = null)
    {
        var query = context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Include(p => p.Reviews)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(p => p.Name.ToLower().Contains(searchTerm.ToLower()));

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId);

        return await query.ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Id == id);
    }


}
