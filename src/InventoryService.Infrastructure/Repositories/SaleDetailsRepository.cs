
using InventoryService.Domain.Entities.Sales;
using InventoryService.Domain.Interfaces;
using InventoryService.Infrastructure.Data;

namespace InventoryService.Infrastructure.Repositories;

public class SaleDetailsRepository : Repository<SaleDetails>, ISaleDetailsRepository
{
    public SaleDetailsRepository(InventoryDbContext context) : base(context)
    {
    }
}
