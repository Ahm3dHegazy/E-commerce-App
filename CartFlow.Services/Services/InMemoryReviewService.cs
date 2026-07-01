using CartFlow.Services.Interfaces;
using CartFlow.Services.Models;

namespace CartFlow.Services.Services;

/// <summary>
/// Ephemeral in-memory review service used for development and testing.
/// Produces deterministic mock reviews by product id so UI looks realistic.
/// Replace with a DB-backed implementation later.
/// </summary>
public class InMemoryReviewService : IReviewService
{
    public Task<List<ReviewDto>> GetReviewsForProductAsync(int productId)
    {
        var rng = new Random(productId * 97 + 13);
        var names = new[] { "Amina", "Hassan", "Layla", "Omar", "Sara", "Youssef", "Mona", "Karim" };
        var comments = new[] {
            "Excellent product, highly recommend!",
            "Good value for money.",
            "Not what I expected, quality could be better.",
            "Fast shipping and works as described.",
            "Five stars — will buy again.",
            "Decent but arrived with minor scratches.",
            "Amazing customer service and product.",
            "Average — acceptable for the price."
        };

        var list = new List<ReviewDto>();
        var count = rng.Next(2, 6);

        for (int i = 0; i < count; i++)
        {
            var name = names[rng.Next(names.Length)];
            var rate = rng.Next(1, 6);
            var comment = comments[rng.Next(comments.Length)];
            var daysAgo = rng.Next(1, 90);

            list.Add(new ReviewDto {
                UserName = name,
                Rate = rate,
                Comment = comment,
                CreatedAt = DateTime.UtcNow.AddDays(-daysAgo)
            });
        }

        return Task.FromResult(list);
    }
}
