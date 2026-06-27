using CartFlow.Data.Data;
using CartFlow.Data.Entities;
using CartFlow.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CartFlow.Services.Services;

public class ProductService(AppDbContext context) : IProductService {
    public async Task<List<Product>> GetFeaturedAsync(int count) {
        return await context.Products
            .Include(p => p.Category)
            .OrderBy(p => EF.Functions.Random())
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<Product>> GetAllAsync() {
        return await context.Products
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id) {
        return await context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}
