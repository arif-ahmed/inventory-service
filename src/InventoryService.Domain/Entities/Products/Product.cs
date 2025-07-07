using InventoryService.Domain.Interfaces;

namespace InventoryService.Domain.Entities.Products;
public class Product : EntityBase, IAuditable, ISoftDeletable
{
    public int ProductId { get; set; }
    public string Name { get; set; } = default!;
    public string Barcode { get; set; } = default!;
    public decimal Price { get; set; } 
    public decimal StockQty { get; set; }
    public string Category { get; set; } = default!;
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
