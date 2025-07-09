using FluentValidation;
using InventoryService.Domain.Entities.Customers;
using InventoryService.Domain.Interfaces;
using MediatR;

namespace InventoryService.Application.Customers;
public class CreateCustomerCommand : IRequest
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public int LoyaltyPoints { get; set; }
}

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand>
{
    private readonly ICustomerRepository _customerRepository;
    public CreateCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }
    public async Task Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = new Customer
        {
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            LoyaltyPoints = request.LoyaltyPoints,
        };

        await _customerRepository.AddAsync(customer, cancellationToken);
    }
}

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(c => c.FullName).NotEmpty().WithMessage("Full name is required.");
        RuleFor(c => c.Email).NotEmpty().EmailAddress().WithMessage("A valid email is required.");
        RuleFor(c => c.Phone).NotEmpty().WithMessage("Phone number is required.");
        RuleFor(c => c.LoyaltyPoints).GreaterThanOrEqualTo(0).WithMessage("Loyalty points cannot be negative.");
    }
}

