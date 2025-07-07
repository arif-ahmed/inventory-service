using InventoryService.Domain.Entities.Customers;
using InventoryService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Data;
public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges()
    {
        ApplyAuditInfo();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditInfo()
    {
        var entries = ChangeTracker.Entries<IAuditable>();

        foreach (var entry in entries)
        {
            var now = DateTime.UtcNow;

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                // entry.Entity.CreatedBy = _currentUser;
                entry.Entity.ModifiedAt = now;
                // entry.Entity.ModifiedBy = _currentUser;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedAt = now;
                // entry.Entity.ModifiedBy = _currentUser;
            }
        }
    }

    // DbSet properties for your entities
    public DbSet<Customer> Customers { get; set; }
}
