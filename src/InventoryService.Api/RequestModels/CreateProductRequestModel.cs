namespace InventoryService.Api.RequestModels;

public class CreateProductRequestModel
{
    public string Name { get; set; } = default!;
    public string Barcode { get; set; } = default!;
    public decimal Price { get; set; }
    public decimal StockQty { get; set; }
    public string Category { get; set; } = default!;
}
