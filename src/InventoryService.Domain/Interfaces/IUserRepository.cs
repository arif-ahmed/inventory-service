using InventoryService.Domain.Entities.Identity;

namespace InventoryService.Domain.Interfaces;
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
}
