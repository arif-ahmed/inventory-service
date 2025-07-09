namespace InventoryService.Application.Dtos;
public class CustomerDto
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public int LoyaltyPoints { get; set; }
}
