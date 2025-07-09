
namespace InventoryService.Domain.Interfaces;

public interface ITokenService
{
    Task<string> GenerateTokenAsync();
}
