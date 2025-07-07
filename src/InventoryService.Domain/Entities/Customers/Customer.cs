using InventoryService.Domain.Entities.Sales;
using InventoryService.Domain.Interfaces;

namespace InventoryService.Domain.Entities.Customers;
public class Customer : ISoftDeletable
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Email { get; set; } = default!;
    public int LoyaltyPoints { get; set; }
    public bool IsDeleted { get; set; }

    // Navigation properties
    public virtual ICollection<Sale> Sales { get; set; } = default!;
}

