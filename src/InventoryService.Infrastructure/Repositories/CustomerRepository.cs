using InventoryService.Domain.Entities.Customers;
using InventoryService.Domain.Interfaces;
using InventoryService.Infrastructure.Data;

namespace InventoryService.Infrastructure.Repositories;
public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(InventoryDbContext context) : base(context) 
    {
    }
}
