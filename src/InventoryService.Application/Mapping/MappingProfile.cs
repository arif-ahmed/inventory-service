using AutoMapper;
using InventoryService.Application.Dtos;
using InventoryService.Domain.Entities.Customers;

namespace InventoryService.Application.Mapping;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Customer, CustomerDto>();
    }
}
