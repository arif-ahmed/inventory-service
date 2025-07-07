using InventoryService.Domain.Entities.Products;
using InventoryService.Infrastructure.Data;

namespace InventoryService.Infrastructure.Repositories;
public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(InventoryDbContext context) : base(context)
    {
    }
}
