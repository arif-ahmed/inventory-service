using InventoryService.Domain.Entities;
using System.Linq.Expressions;

namespace InventoryService.Domain.Interfaces;
public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(IEnumerable<TEntity>, int)> FindAsync(Expression<Func<TEntity, bool>> predicate, int offset, int page, CancellationToken cancellationToken = default);
    Task<IQueryable<TEntity>> GetQueryable();
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}

