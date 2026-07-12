using CartFlow.Data.Data;
using CartFlow.Data.Entities;
using CartFlow.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CartFlow.Services.Services;

public class CartService(AppDbContext context) : ICartService
{
    public async Task<List<CartItem>> GetCartItemsAsync()
    {
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

    public async Task MergeGuestCartAsync(int userId, List<CartFlow.Services.Interfaces.GuestCartItemDto> guestItems)
    {
        if (guestItems == null || !guestItems.Any()) return;

        // Ensure user's cart exists
        var cart = await context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            context.Carts.Add(cart);
            await context.SaveChangesAsync();
        }

        foreach (var gi in guestItems)
        {
            // Validate product still exists
            var product = await context.Products.FindAsync(gi.ProductId);
            if (product == null) continue;

            var existing = await context.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == gi.ProductId);
            if (existing != null)
            {
                existing.Quantity += gi.Quantity;
                existing.UnitPrice = product.UnitPrice;
            }
            else
            {
                var newItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = gi.ProductId,
                    Quantity = gi.Quantity,
                    UnitPrice = product.UnitPrice
                };
                context.CartItems.Add(newItem);
            }
        }

        await context.SaveChangesAsync();
    }
}
