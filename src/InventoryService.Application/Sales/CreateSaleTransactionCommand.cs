using InventoryService.Domain.Entities.Sales;
using InventoryService.Domain.Interfaces;
using InventoryService.Domain.Interfaces.Pricing;
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
    private IDiscountPolicy _discountPolicy;
    private IVATPolicy _vatPolicy;
    public CreateSaleTransactionCommandHandler(IProductRepository productRepository, ISalesRepository salesRepository, ISaleDetailsRepository saleDetailsRepository, IDiscountPolicy discountPolicy, IVATPolicy vatPolicy)
    {
        _productRepository = productRepository;
        _salesRepository = salesRepository;
        _saleDetailsRepository = saleDetailsRepository;
        _discountPolicy = discountPolicy;
        _vatPolicy = vatPolicy;
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

        decimal subTotal = 0;


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
            subTotal += detail.Quantity * detail.Price; // Update total amount

            await _productRepository.UpdateAsync(product, cancellationToken); 
            await _saleDetailsRepository.AddAsync(saleDetail, cancellationToken);
        }

        var discountAmount = _discountPolicy.ApplyDiscount(subTotal, 0, 10m); // Apply discount policy
        subTotal -= discountAmount;
        
        var vatAmount = _vatPolicy.CalculateVAT(subTotal, 15m); // Apply VAT policy
        subTotal += vatAmount; // Add VAT to total amount

        sale.TotalAmount = subTotal; 
        sale.DueAmount = sale.TotalAmount - sale.PaidAmount; // Calculate due amount
        await _salesRepository.AddAsync(sale, cancellationToken);

        // uow save changes
        await Task.Delay(3000);
    }
}
