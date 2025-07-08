namespace InventoryService.Api.RequestModels;

public class ProductUpdateDto
{
    public string? Name { get; set; }
    public string? Barcode { get; set; }
    public string? Category { get; set; }
    public decimal? Price { get; set; }
    public int? StockQty { get; set; }
}
