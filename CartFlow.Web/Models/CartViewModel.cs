namespace CartFlow.Web.Models
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new();

        public decimal Shipping { get; set; } = 5.00m;

        public decimal Subtotal => Items.Sum(item => item.LineTotal);

        public decimal Total => Subtotal + Shipping;
    }
}
