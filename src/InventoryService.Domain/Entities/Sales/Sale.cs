using InventoryService.Domain.Entities.Customers;
using System.ComponentModel.DataAnnotations;

namespace InventoryService.Domain.Entities.Sales;
public class Sale
{
    [Key]
    public int SaleId { get; set; }
    public DateTime SaleDate { get; set; }
    public int? CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<SaleDetails> SaleDetails { get; set; } = new List<SaleDetails>();
}
