namespace InventoryService.Application.Dtos;
public class CustomerDto
{
    public int CustomerId { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int? LoyaltyPoints { get; set; }
}
