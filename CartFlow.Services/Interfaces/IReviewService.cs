using CartFlow.Services.Models;

namespace CartFlow.Services.Interfaces;

public interface IReviewService
{
    /// <summary>
    /// Get reviews for a product. Implementation may be ephemeral or backed by a real store.
    /// Returns DTOs defined in the Services layer to avoid circular dependencies.
    /// </summary>
    Task<List<ReviewDto>> GetReviewsForProductAsync(int productId);
}
