using InventoryService.Domain.Entities.Products;
using InventoryService.Domain.Interfaces;
using MediatR;

namespace InventoryService.Application.Products;
public class CreateProductCommand : IRequest
{
    public string Name { get; set; } = default!;
    public string Barcode { get; set; } = default!;
    public decimal Price { get; set; }
    public decimal StockQty { get; set; }
    public string Category { get; set; } = default!;
    // public bool Status { get; set; }
    // public bool IsDeleted { get; set; }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand>
{
    private readonly IProductRepository _productRepository;
    public CreateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    public async Task Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Barcode = request.Barcode,
            Price = request.Price,
            StockQty = request.StockQty,
            Category = request.Category,
            Status = true
            // Status = request.Status,
            // IsDeleted = request.IsDeleted
        };

        await _productRepository.AddAsync(product, cancellationToken);
    }
}

