using CartFlow.Data.Entities;

namespace CartFlow.Web.Models
{
    /// <summary>
    /// Passed to the Products/Index view.
    /// Carries the paged/filtered product list plus the data needed to re-render the filter bar.
    /// </summary>
    public class ProductListViewModel
    {
        public List<ProductViewModel> Products { get; set; } = new();

        // Filter state — preserved so the view can re-populate the search box and category dropdown
        public string? SearchTerm { get; set; }
        public int? SelectedCategoryId { get; set; }

        // Populated from the Categories table; used to render the category filter dropdown
        public List<Category> Categories { get; set; } = new();
    }
}
