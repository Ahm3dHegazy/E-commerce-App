namespace CartFlow.Web.Models;

public class MyOrdersViewModel
{
    public List<OrderListItem> Orders { get; set; } = new();
}

public class OrderListItem
{
    public int Id { get; set; }
    public string OrderNumber => Id.ToString("D6");
    public DateTime OrderDate { get; set; }
    public int ItemCount { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
}
