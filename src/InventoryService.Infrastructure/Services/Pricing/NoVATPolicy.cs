using InventoryService.Domain.Interfaces.Pricing;

namespace InventoryService.Infrastructure.Services.Pricing;

public class NoVATPolicy : IVATPolicy
{
    public decimal CalculateVAT(decimal amountAfterDiscount, decimal vatPercent)
        => 0;
}
