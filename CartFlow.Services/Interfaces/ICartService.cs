using CartFlow.Data.Entities;

namespace CartFlow.Services.Interfaces;

public interface ICartService
{
    Task<List<CartItem>> GetCartItemsAsync();
    Task ClearCartAsync();
    Task MergeGuestCartAsync(int userId, List<GuestCartItemDto> guestItems);
}
