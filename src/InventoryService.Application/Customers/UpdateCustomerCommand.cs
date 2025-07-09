using FluentValidation;
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

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(c => c.CustomerId)
            .GreaterThan(0)
            .WithMessage("Customer ID must be greater than 0.");

        RuleFor(c => c.FullName)
            .NotEmpty()
            .When(c => c.FullName != null)
            .WithMessage("Full name is required if provided.");

        RuleFor(c => c.Email)
            .EmailAddress()
            .When(c => !string.IsNullOrWhiteSpace(c.Email))
            .WithMessage("A valid email is required if provided.");

        RuleFor(c => c.Phone)
            .NotEmpty()
            .When(c => c.Phone != null)
            .WithMessage("Phone number is required if provided.");

        RuleFor(c => c.LoyaltyPoints)
            .GreaterThanOrEqualTo(0)
            .When(c => c.LoyaltyPoints.HasValue)
            .WithMessage("Loyalty points cannot be negative.");
    }
}
