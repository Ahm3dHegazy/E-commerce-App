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
}
