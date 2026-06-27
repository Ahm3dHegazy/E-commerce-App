using CartFlow.Data.Data;
using CartFlow.Data.Entities;
using CartFlow.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CartFlow.Services.Services;

public class CategoryService(AppDbContext context) : ICategoryService {
    public async Task<List<Category>> GetAllWithHierarchyAsync() {
        return await context.Categories
            .Include(c => c.Subcategories)
                .ThenInclude(sc => sc.Products)
            .Include(c => c.Products)
            .Where(c => c.ParentCategoryId == null)
            .ToListAsync();
    }
}
