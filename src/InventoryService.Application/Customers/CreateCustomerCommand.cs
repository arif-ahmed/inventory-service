using InventoryService.Domain.Entities.Customers;
using InventoryService.Domain.Interfaces;
using MediatR;

namespace InventoryService.Application.Customers;
public class CreateCustomerCommand : IRequest<int>
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Address { get; set; } = default!;
}

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, int>
{
    private readonly ICustomerRepository _customerRepository;
    //public CreateCustomerCommandHandler(ICustomerRepository customerRepository)
    //{
    //    _customerRepository = customerRepository;
    //}
    public async Task<int> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = new Customer
        {
            Name = request.Name,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address
        };
        // await _customerRepository.AddAsync(customer);
        // return customer.CustomerId; 
        return await Task.FromResult(1); // Simulating async operation for example purposes
    }
}

