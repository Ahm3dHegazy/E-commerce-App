using CartFlow.Data.Entities;

namespace CartFlow.Services.Interfaces;

public interface ICartService
{
    Task<List<CartItem>> GetCartItemsAsync();
    Task ClearCartAsync();
}
