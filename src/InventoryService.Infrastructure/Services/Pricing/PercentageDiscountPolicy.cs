using InventoryService.Domain.Interfaces.Pricing;

namespace InventoryService.Infrastructure.Services.Pricing;
public class PercentageDiscountPolicy : IDiscountPolicy
{
    public decimal ApplyDiscount(decimal subTotal, decimal discountAmount, decimal discountPercent)
        => subTotal * (discountPercent / 100.0m);
}

