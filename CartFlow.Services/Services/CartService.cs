using CartFlow.Data.Data;
using CartFlow.Data.Entities;
using CartFlow.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CartFlow.Services.Services;

public class CartService(AppDbContext context) : ICartService {
    public async Task<List<CartItem>> GetCartItemsAsync() {
        return await context.CartItems
            .Include(ci => ci.Product)
                .ThenInclude(p => p.Category)
            .ToListAsync();
    }

    public async Task ClearCartAsync()
    {
        var items = await context.CartItems.ToListAsync();
        context.CartItems.RemoveRange(items);
        await context.SaveChangesAsync();
    }
}
