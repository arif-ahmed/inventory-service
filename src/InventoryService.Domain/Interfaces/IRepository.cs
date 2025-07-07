using InventoryService.Domain.Entities;
using System.Linq.Expressions;

namespace InventoryService.Domain.Interfaces;
public interface IRepository<TEntity> where TEntity : EntityBase
{
    Task<TEntity?> GetByIdAsync(string id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, string sortBy, string sortOrder, int offset, int page);
    Task<int> AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(string id);
}

