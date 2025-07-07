using InventoryService.Domain.Entities.Products;

namespace InventoryService.Domain.Entities.Sales;

public class SaleDetails
{
    public int SaleDetailId { get; set; }
    public int SaleId { get; set; }
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public Sale Sale { get; set; } = default!;
    public Product Product { get; set; } = default!;
}
