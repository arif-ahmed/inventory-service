using AutoMapper;
using InventoryService.Application.Dtos;
using InventoryService.Domain.Interfaces;
using MediatR;

namespace InventoryService.Application.Products;

public class GetProductsQuery : IRequest<IEnumerable<ProductDto>>
{
}

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    public GetProductsQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    public async Task<IEnumerable<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync();
        if (products == null || !products.Any())
        {
            return Enumerable.Empty<ProductDto>();
        }
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }
}
