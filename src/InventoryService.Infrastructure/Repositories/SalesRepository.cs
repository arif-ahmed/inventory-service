
using InventoryService.Domain.Entities.Sales;
using InventoryService.Domain.Interfaces;
using InventoryService.Infrastructure.Data;

namespace InventoryService.Infrastructure.Repositories;

public class SalesRepository : Repository<Sale>, ISalesRepository
{
    public SalesRepository(InventoryDbContext context) : base(context)
    {

    }

    public override Task<Sale?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
