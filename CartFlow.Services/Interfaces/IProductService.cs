using CartFlow.Data.Entities;

namespace CartFlow.Services.Interfaces;

public interface IProductService
{

    Task<List<Product>> GetFeaturedAsync(int count);
    Task<List<Product>> GetAllAsync(string? searchTerm = null, int? categoryId = null);
    Task<Product?> GetByIdAsync(int id);
    Task<List<Product>> GetRelatedAsync(int productId, int categoryId, int count);

}
