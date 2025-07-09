
using InventoryService.Domain.Entities.Identity;
using InventoryService.Domain.Interfaces;
using InventoryService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InventoryService.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(InventoryDbContext context) : base(context)
    {

    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        return DbSet.FirstOrDefaultAsync(user => user.Username == username);
    }
}
