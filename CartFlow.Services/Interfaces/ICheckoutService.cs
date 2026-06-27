namespace CartFlow.Services.Interfaces;

public class CheckoutData {
    public string OrderNumber { get; set; } = "";
    public int ItemCount { get; set; }
    public decimal Total { get; set; }
}

public interface ICheckoutService {
    CheckoutData GetConfirmation();
}
