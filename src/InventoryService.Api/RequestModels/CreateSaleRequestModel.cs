namespace InventoryService.Api.RequestModels;

public class CreateSaleRequestModel
{
    public int SaleId { get; set; }
    public DateTime SaleDate { get; set; }
    public int? CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public List<SaleDetailDto> SaleDetails { get; set; } = default!;
}

public class SaleDetailDto
{
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
}
