using InventoryService.Domain.Entities;
using InventoryService.Domain.Interfaces;
using InventoryService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InventoryService.Infrastructure.Repositories;
public class Repository<TEntity> : IRepository<TEntity> where TEntity : EntityBase
{
    private readonly InventoryDbContext _context;
    protected DbSet<TEntity> DbSet => _context.Set<TEntity>();
    public Repository(InventoryDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<int> AddAsync(TEntity entity)
    {
        await DbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity.Id; // Assuming Id is the primary key and is set after saving
    }

    public Task DeleteAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id), "Id cannot be null or empty.");
        }
        var entity = DbSet.Find(id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Entity with id {id} not found.");
        }
        DbSet.Remove(entity);
        return _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, string sortBy, string sortOrder, int offset, int page)
    {
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate), "Predicate cannot be null.");
        }
        var query = DbSet.Where(predicate);
        if (!string.IsNullOrEmpty(sortBy))
        {
            query = sortOrder.ToLower() == "desc" ? 
                query.OrderByDescending(e => EF.Property<object>(e, sortBy)) : 
                query.OrderBy(e => EF.Property<object>(e, sortBy));
        }
        if (offset > 0)
        {
            query = query.Skip(offset);
        }
        if (page > 0)
        {
            query = query.Take(page);
        }
        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        var entities = await DbSet.AsNoTracking().ToListAsync();
        return entities;
    }

    public async Task<TEntity?> GetByIdAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id), "Id cannot be null or empty.");
        }
        return await DbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id.ToString() == id);
    }

    public async Task UpdateAsync(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        // Find key property info safely
        var entityType = _context.Model.FindEntityType(typeof(TEntity));
        if (entityType == null)
            throw new InvalidOperationException($"Could not find entity type {typeof(TEntity).Name} in the context.");

        var keyProperty = entityType.FindPrimaryKey()?.Properties?.FirstOrDefault();
        if (keyProperty == null)
            throw new InvalidOperationException($"No primary key defined for entity type {typeof(TEntity).Name}.");

        var keyName = keyProperty.Name;
        var keyPropInfo = typeof(TEntity).GetProperty(keyName);
        if (keyPropInfo == null)
            throw new InvalidOperationException($"Entity type {typeof(TEntity).Name} does not have a property named {keyName}.");

        var keyValue = keyPropInfo.GetValue(entity);
        if (keyValue == null)
            throw new InvalidOperationException("Key value cannot be null.");

        // Retrieve the existing entity from the database
        var existingEntity = await DbSet.FindAsync(keyValue);
        if (existingEntity == null)
            throw new InvalidOperationException("Entity not found in database.");

        // Iterate over properties and update values
        foreach (var property in typeof(TEntity).GetProperties())
        {
            // Skip key and non-writable properties
            if (property.Name == keyName || !property.CanWrite)
                continue;

            var newValue = property.GetValue(entity);
            var oldValue = property.GetValue(existingEntity);

            // Only update if different
            if (!Equals(newValue, oldValue))
            {
                property.SetValue(existingEntity, newValue);
            }
        }

        await _context.SaveChangesAsync();
    }
}
