namespace InventoryService.Domain.Interfaces.Pricing;
public interface IDiscountPolicy
{
    decimal ApplyDiscount(decimal subTotal, decimal discountAmount, decimal discountPercent);
}
