using AutoMapper;
using InventoryService.Application.Dtos;
using InventoryService.Domain.Interfaces;
using MediatR;

namespace InventoryService.Application.Products;

public class GetProductsQuery : IRequest<(IEnumerable<ProductDto>, int)>
{
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10; 
}

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, (IEnumerable<ProductDto>, int)>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    public GetProductsQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    public async Task<(IEnumerable<ProductDto>, int)> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        //if (!string.IsNullOrEmpty(request.SearchTerm))
        //{
        //    await _productRepository.FindAsync(request.PageNumber, request.PageSize, cancellationToken);
        //}

        //var result = await _productRepository.FindAsync(product => product.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase), request.PageNumber, request.PageSize, cancellationToken);
        //if (result.Item1 == null || !result.Item1.Any())
        //{
        //    return (Enumerable.Empty<ProductDto>(), 0);
        //}
        //var productsDto = _mapper.Map<IEnumerable<ProductDto>>(result.Item1);
        //return (productsDto, result.Item2);

        var query = await _productRepository.GetQueryable();

        if(!string.IsNullOrEmpty(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim().ToLower();
            query = query.Where(product =>
                product.Name.ToLower().Contains(term) ||
                product.Barcode.ToLower().Contains(term) ||
                product.Category.ToLower().Contains(term) 
            );
        }

        int totalCount = query.Count();

        // Pagination
        if (request.PageNumber > 0)
            query = query.Skip(request.PageNumber);

        if (request.PageSize > 0)
            query = query.Take(request.PageSize);

        var data = query.ToList();

        var dtoList = _mapper.Map<IEnumerable<ProductDto>>(data);
        return (dtoList, totalCount);
    }
}
