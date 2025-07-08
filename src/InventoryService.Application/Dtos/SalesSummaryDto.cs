namespace InventoryService.Application.Dtos;

public class SalesSummaryDto
{
    public decimal TotalSales { get; set; }
    public decimal TotalRevenue { get; set; }
    public int NumberOfTransactions { get; set; }
}
