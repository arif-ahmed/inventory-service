using InventoryService.Domain.Entities.Identity;

namespace InventoryService.Domain.Interfaces;
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> IsUsernameTakenAsync(string username);
    Task<bool> IsEmailTakenAsync(string email);
    Task<User?> AuthenticateAsync(string username, string password);
}
