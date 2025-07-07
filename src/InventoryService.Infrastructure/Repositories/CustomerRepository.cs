using InventoryService.Domain.Entities.Customers;
using InventoryService.Domain.Interfaces;
using InventoryService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Repositories;
public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(InventoryDbContext context) : base(context) 
    {
    }

    public override async Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");
        }

        return await DbSet.FirstOrDefaultAsync(customer => customer.CustomerId == id && !customer.IsDeleted);
    }

    public override async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(customer => !customer.IsDeleted).ToListAsync();
    }
}
