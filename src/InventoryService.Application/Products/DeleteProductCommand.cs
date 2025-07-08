
using InventoryService.Domain.Interfaces;
using MediatR;

namespace InventoryService.Application.Products;

public class DeleteProductCommand : IRequest
{
    public required int ProductId { get; set; }
}

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IProductRepository _productRepository;
    public DeleteProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }
    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID {request.ProductId} not found.");
        }
        product.IsDeleted = true;
        await _productRepository.UpdateAsync(product);
    }
}
