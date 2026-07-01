namespace CartFlow.Web.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int StockQuantity { get; set; }

        // Denormalised from the related Category entity
        public string? CategoryName { get; set; }
        public int CategoryId { get; set; }

        // URL of the first ProductImage (null when no images exist)
        public string? ImageUrl { get; set; }

        // First letter of the product name — used as a fallback avatar/placeholder in the UI
        public string Initial { get; set; } = string.Empty;

        // Reviews (populated from IReviewService). Uses DTOs from Services layer to avoid circular deps.
        public List<CartFlow.Services.Models.ReviewDto> Reviews { get; set; } = new();

        // Convenience properties for the index view
        public decimal AverageRating => Reviews?.Any() == true ? Math.Round(Reviews.Average(r => r.Rate), 2) : 0m;
        public int ReviewCount => Reviews?.Count() ?? 0;

        // Calculated average rating based on Reviews
    }
}
