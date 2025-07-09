
namespace InventoryService.Domain.Interfaces;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(string username, string role, int userId);
}
