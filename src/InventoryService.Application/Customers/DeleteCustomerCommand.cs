
using InventoryService.Domain.Interfaces;
using MediatR;

namespace InventoryService.Application.Customers;

public class DeleteCustomerCommand : IRequest
{
    public required int CustomerId { get; set; }
}

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand>
{
    private readonly ICustomerRepository _customerRepository;
    public DeleteCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
    }
    public async Task Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
        if (customer == null)
        {
            // throw new NotFoundException($"Customer with ID {request.CustomerId} not found.");
            throw new Exception($"Customer with ID {request.CustomerId} not found.");
        }
        customer.IsDeleted = true;
        await _customerRepository.UpdateAsync(customer);
    }
}
