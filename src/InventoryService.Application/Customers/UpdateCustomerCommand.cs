using InventoryService.Domain.Interfaces;
using MediatR;

namespace InventoryService.Application.Customers;

public class UpdateCustomerCommand : IRequest<string>
{
    public int CustomerId { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int? LoyaltyPoints { get; set; }
}

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, string>
{
    private readonly ICustomerRepository _customerRepository;
    public UpdateCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }
    public async Task<string> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
        if (customer == null)
        {
            throw new Exception($"Customer with ID {request.CustomerId} not found.");
        }

        if (request.FullName != null)
            customer.FullName = request.FullName;

        if (request.Email != null)
            customer.Email = request.Email;

        if (request.Phone != null)
            customer.Phone = request.Phone;

        if (request.LoyaltyPoints.HasValue)
            customer.LoyaltyPoints = request.LoyaltyPoints.Value;

        await _customerRepository.UpdateAsync(customer);
        return "Customer updated successfully";
    }
}
