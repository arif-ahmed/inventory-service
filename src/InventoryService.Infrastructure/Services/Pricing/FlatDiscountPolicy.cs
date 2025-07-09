using InventoryService.Domain.Interfaces.Pricing;

namespace InventoryService.Infrastructure.Services.Pricing;
public class FlatDiscountPolicy : IDiscountPolicy
{
    public decimal ApplyDiscount(decimal subTotal, decimal discountAmount, decimal discountPercent)
        => discountAmount;
}
