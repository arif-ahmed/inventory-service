using InventoryService.Domain.Interfaces;
using InventoryService.Infrastructure.Data;
using InventoryService.Infrastructure.Repositories;
using System;

namespace InventoryService.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly InventoryDbContext _context;
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(InventoryDbContext context)
    {
        _context = context;
    }

    public IRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T);
        if (!_repositories.ContainsKey(type))
        {
            // _repositories[type] = new Repository<T>(_context);
        }
        return (IRepository<T>)_repositories[type];
    }

    public async Task<int> CommitAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
