namespace InventoryService.Domain.Interfaces.Pricing;
public interface IVATPolicy
{
    decimal CalculateVAT(decimal amountAfterDiscount, decimal vatPercent);
}
