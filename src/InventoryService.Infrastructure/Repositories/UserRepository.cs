
using InventoryService.Domain.Entities.Identity;
using InventoryService.Domain.Interfaces;
using InventoryService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InventoryService.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly InventoryDbContext _context;
    public UserRepository(InventoryDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public Task AddAsync(User entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<User?> AuthenticateAsync(string username, string password)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<(IEnumerable<User>, int)> FindAsync(Expression<Func<User, bool>> predicate, int offset, int page, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        // throw new NotImplementedException();
        return _context.Users.FirstOrDefaultAsync(user => user.Email == email);
    }

    public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        return _context.Users.FirstOrDefaultAsync(user => user.Username == username);
    }

    public Task<IQueryable<User>> GetQueryable()
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsEmailTakenAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsUsernameTakenAsync(string username)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(User entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
