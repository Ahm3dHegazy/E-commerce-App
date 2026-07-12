using CartFlow.Data.Entities;

namespace CartFlow.Services.Interfaces;

public interface ICategoryService
{
    Task<List<Category>> GetAllWithHierarchyAsync();
}
