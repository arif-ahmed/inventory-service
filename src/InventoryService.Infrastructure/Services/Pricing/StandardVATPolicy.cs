using InventoryService.Domain.Interfaces.Pricing;

namespace InventoryService.Infrastructure.Services.Pricing;
public class StandardVATPolicy : IVATPolicy
{
    public decimal CalculateVAT(decimal amountAfterDiscount, decimal vatPercent)
        => amountAfterDiscount * (vatPercent / 100.0m);
}
