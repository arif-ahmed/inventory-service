using InventoryService.Domain.Entities.Products;
using InventoryService.Domain.Interfaces;
using InventoryService.Infrastructure.Data;

namespace InventoryService.Infrastructure.Repositories;
public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(InventoryDbContext context) : base(context)
    {
    }

    public override async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");
        }

        return await DbSet.FindAsync(new object[] { id }, cancellationToken);
    }
}
