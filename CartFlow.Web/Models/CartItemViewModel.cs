namespace CartFlow.Web.Models
{
    public class CartItemViewModel
    {
        public int Id { get; set; }
        public int CartItemId => Id;
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public decimal LineTotal => UnitPrice * Quantity;
    }
}