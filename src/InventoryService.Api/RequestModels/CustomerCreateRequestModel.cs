namespace InventoryService.Api.RequestModels;

public class CustomerCreateRequestModel
{
    public string FullName { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Email { get; set; } = default!;
    public int LoyaltyPoints { get; set; }
}
