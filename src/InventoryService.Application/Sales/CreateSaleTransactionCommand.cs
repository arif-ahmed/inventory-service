using InventoryService.Domain.Entities.Sales;
using InventoryService.Domain.Interfaces;
using MediatR;

namespace InventoryService.Application.Sales;
public class CreateSaleTransactionCommand : IRequest
{
    public DateTime SaleDate { get; set; }
    public int? CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public List<SaleDetailDto> SaleDetails { get; set; } = new();
}

public class SaleDetailDto
{
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
}

public class CreateSaleTransactionCommandHandler : IRequestHandler<CreateSaleTransactionCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly ISalesRepository _salesRepository;
    private readonly ISaleDetailsRepository _saleDetailsRepository;
    public CreateSaleTransactionCommandHandler(IProductRepository productRepository, ISalesRepository salesRepository, ISaleDetailsRepository saleDetailsRepository)
    {
        _productRepository = productRepository;
        _salesRepository = salesRepository;
        _saleDetailsRepository = saleDetailsRepository;
    }
    public async Task Handle(CreateSaleTransactionCommand request, CancellationToken cancellationToken)
    {
        var sale = new Sale
        {
            SaleDate = request.SaleDate,
            CustomerId = request.CustomerId,
            TotalAmount = request.TotalAmount,
            PaidAmount = request.PaidAmount,
            DueAmount = request.DueAmount
        };


        foreach (var detail in request.SaleDetails)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(detail.ProductId, cancellationToken);

            if (product == null)
            {
                throw new ArgumentException($"Product with ID {detail.ProductId} does not exist.");
            }

            // Check if product has sufficient stock
            if (product.StockQty < detail.Quantity)
            {
                throw new InvalidOperationException($"Insufficient stock for product {product.Name}. Available: {product.StockQty}, Requested: {detail.Quantity}");
            }

            var saleDetail = new SaleDetails
            {
                SaleId = sale.SaleId,
                ProductId = detail.ProductId,
                Quantity = detail.Quantity,
                Price = detail.Price
            };

            product.StockQty -= detail.Quantity; // Deduct stock
            sale.TotalAmount += detail.Quantity * detail.Price; // Update total amount

            await _productRepository.UpdateAsync(product, cancellationToken); 
            await _saleDetailsRepository.AddAsync(saleDetail, cancellationToken);
        }

        sale.DueAmount = sale.TotalAmount - sale.PaidAmount; // Calculate due amount
        await _salesRepository.AddAsync(sale, cancellationToken);

        // uow save changes
        await Task.Delay(3000);
    }
}
