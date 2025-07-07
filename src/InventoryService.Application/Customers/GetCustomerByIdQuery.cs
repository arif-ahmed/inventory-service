using AutoMapper;
using InventoryService.Application.Dtos;
using InventoryService.Domain.Interfaces;
using MediatR;

namespace InventoryService.Application.Customers;
public class GetCustomerByIdQuery : IRequest<CustomerDto>
{
    public int Id { get; }
    public GetCustomerByIdQuery(int id)
    {
        Id = id;
    }
}

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;
    public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        _mapper = mapper;
    }
    public async Task<CustomerDto> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id);
        return _mapper.Map<CustomerDto>(customer);
    }
}

