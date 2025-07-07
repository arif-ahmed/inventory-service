using InventoryService.Domain.Entities.Customers;
using InventoryService.Domain.Interfaces;

namespace InventoryService.Domain.Entities.Sales;
public class Sale : EntityBase
{
    public int SaleId { get; set; }
    public DateTime SaleDate { get; set; }
    public int? CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<SaleDetails> SaleDetails { get; set; } = new List<SaleDetails>();

    //public int SaleId { get; set; }
    //public string ProductId { get; set; } = default!;
    //public string CustomerId { get; set; } = default!;
    //public decimal SalePrice { get; set; }
    //public int Quantity { get; set; }
    //public DateTime SaleDate { get; set; }
    //public bool IsActive { get; set; }
    //public bool IsDeleted { get; set; }
    //public DateTime CreatedAt { get; set; }
    //public string? CreatedBy { get; set; }
    //public DateTime? ModifiedAt { get; set; }
    //public string? ModifiedBy { get; set; }
    //public DateTime? DeletedAt { get; set; }
    //public string? DeletedBy { get; set; }
}
