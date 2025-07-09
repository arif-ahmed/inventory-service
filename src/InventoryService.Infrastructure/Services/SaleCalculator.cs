using InventoryService.Domain.Interfaces.Pricing;

namespace InventoryService.Infrastructure.Services;

public class SaleCalculator
{
    private readonly IDiscountPolicy _discountPolicy;
    private readonly IVATPolicy _vatPolicy;

    public SaleCalculator(IDiscountPolicy discountPolicy, IVATPolicy vatPolicy)
    {
        _discountPolicy = discountPolicy;
        _vatPolicy = vatPolicy;
    }

    public (decimal total, decimal discount, decimal vat) CalculateTotal(decimal subTotal, decimal discountAmount, decimal vatAmount)
    {
        //var discount = _discountPolicy.ApplyDiscount(subTotal, discountAmount);
        //var afterDiscount = Math.Max(subTotal - discount, 0);
        //var vat = _vatPolicy.CalculateVAT(afterDiscount, vatAmount);
        //var total = afterDiscount + vat;
        //return (total, discount, vat);

        return (0,0, 0);
    }
}

