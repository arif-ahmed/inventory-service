
namespace InventoryService.Application.Dtos;

public class ProductDto
{
    public int ProductId { get; set; }
    public string Name { get; set; } = default!;
    public string Barcode { get; set; } = default!;
    public decimal Price { get; set; }
    public decimal StockQty { get; set; }
    public string Category { get; set; } = default!;
    public string Status { get; set; } = default!;
    public bool IsDeleted { get; set; }
}
