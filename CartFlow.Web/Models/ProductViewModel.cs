namespace CartFlow.Web.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int StockQuantity { get; set; }

        public string? CategoryName { get; set; }
        public int CategoryId { get; set; }

        public string? ImageUrl { get; set; }
        public List<string> ImageUrls { get; set; } = new();

        public string Initial { get; set; } = string.Empty;

        // القائمة الحقيقية للمراجعات
        public List<CartFlow.Services.Models.ReviewDto> Reviews { get; set; } = new();

        // الخصائص الذكية المحسوبة ديناميكياً تلقائياً بمجرد ملء القائمة أعلاه 🔥
        public decimal AverageRating => Reviews?.Any() == true ? (decimal)Math.Round(Reviews.Average(r => r.Rate), 2) : 0m;
        public int ReviewCount => Reviews?.Count() ?? 0;
    }
}