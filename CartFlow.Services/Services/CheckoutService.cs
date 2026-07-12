using CartFlow.Services.Interfaces;

namespace CartFlow.Services.Services;

public class CheckoutService : ICheckoutService
{
    public CheckoutData GetConfirmation()
    {
        return new CheckoutData
        {
            OrderNumber = "CF-1001",
            ItemCount = 3,
            Total = 64.49m
        };
    }
}
