using CartFlow.Data.Data;
using CartFlow.Data.Entities;
using CartFlow.Services.Interfaces;
using CartFlow.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace CartFlow.Services.Services;

public class ReviewService(AppDbContext context) : IReviewService
{
    public async Task<List<ReviewDto>> GetReviewsForProductAsync(int productId)
    {
        return await context.Reviews
            .AsNoTracking()
            .Where(r => r.ProductId == productId)
            .Include(r => r.User)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewDto
            {
                UserId = r.UserId,
                UserName = r.User.FirstName + " " + r.User.LastName,
                Rate = r.Rate,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }

    public async Task AddReviewAsync(int productId, int userId, int rating, string comment)
    {
        var review = new Review
        {
            ProductId = productId,
            UserId = userId,
            Rate = rating,
            Comment = comment,
            CreatedAt = DateTime.UtcNow
        };

        context.Reviews.Add(review);
        await context.SaveChangesAsync();
    }
}
