using InventoryService.Domain.Interfaces;
using MediatR;

namespace InventoryService.Application.Products;

public class UpdateProductCommand : IRequest<string>
{
    public int ProductId { get; set; }
    public string? Name { get; set; } = default!;
    public string? Barcode { get; set; } = default!;
    public decimal? Price { get; set; }
    public decimal? StockQty { get; set; }
    public string? Category { get; set; } = default!;
    public bool? Status { get; set; }
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, string>
{
    private readonly IProductRepository _productRepository;
    public UpdateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    public async Task<string> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId);

        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID {request.ProductId} not found.");
        }

        product.Name = request.Name ?? product.Name;
        product.Barcode = request.Barcode ?? product.Barcode;
        product.Price = request.Price ?? product.Price;
        product.StockQty = request.StockQty ?? product.StockQty;
        product.Category = request.Category ?? product.Category;
        product.Status = request.Status ?? product.Status;

        await _productRepository.UpdateAsync(product);
        return $"Product with ID {request.ProductId} updated successfully.";
    }
}
