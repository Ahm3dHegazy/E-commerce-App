using CartFlow.Data.Data;
using CartFlow.Data.Entities;
using CartFlow.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CartFlow.Services.Services;

public class ProductService(AppDbContext context) : IProductService
{
    public async Task<List<Product>> GetFeaturedAsync(int count)
    {
        return await context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .OrderBy(p => EF.Functions.Random())
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<Product>> GetAllAsync(string? searchTerm = null, int? categoryId = null)
    {
        var query = context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
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
            .FirstOrDefaultAsync(p => p.Id == id);
    }


}
