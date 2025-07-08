using AutoMapper;
using InventoryService.Application.Dtos;
using InventoryService.Domain.Entities.Customers;
using InventoryService.Domain.Entities.Products;

namespace InventoryService.Application.Mapping;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Customer, CustomerDto>();
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status ? "Available" : "Unavailable"));
    }
}
