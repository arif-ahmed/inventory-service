namespace InventoryService.Application.Dtos;

public class LoginResponse
{
    public bool IsAuthenticated { get; set; }
    public string Token { get; set; } = string.Empty;
    //public string RefreshToken { get; set; } = string.Empty;
    //public DateTime ExpiresAt { get; set; }
    //public DateTime RefreshTokenExpiresAt { get; set; }
}
