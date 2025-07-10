using FluentValidation;
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
        // 1. Create the Sale and save it to generate SaleId
        var sale = new Sale
        {
            SaleDate = request.SaleDate,
            CustomerId = request.CustomerId,
            TotalAmount = request.TotalAmount,
            PaidAmount = request.PaidAmount,
            DueAmount = request.DueAmount
        };

        await _salesRepository.AddAsync(sale, cancellationToken); // SaleId now available if using EF or similar

        decimal subTotal = 0;

        // 2. Process each SaleDetail (check product, update stock, add sale detail)
        foreach (var detail in request.SaleDetails)
        {
            // Check product exists
            var product = await _productRepository.GetByIdAsync(detail.ProductId, cancellationToken);
            if (product == null)
            {
                throw new ArgumentException($"Product with ID {detail.ProductId} does not exist.");
            }

            // Check stock
            if (product.StockQty < detail.Quantity)
            {
                throw new InvalidOperationException($"Insufficient stock for product {product.Name}. Available: {product.StockQty}, Requested: {detail.Quantity}");
            }

            // Deduct stock and update product
            product.StockQty -= detail.Quantity;
            await _productRepository.UpdateAsync(product, cancellationToken);

            // Create sale detail (use valid SaleId)
            var saleDetail = new SaleDetails
            {
                SaleId = sale.SaleId,
                ProductId = detail.ProductId,
                Quantity = detail.Quantity,
                Price = detail.Price
            };

            subTotal += detail.Quantity * detail.Price;
            await _saleDetailsRepository.AddAsync(saleDetail, cancellationToken);
        }

        // 3. Apply discount and VAT
        var discountAmount = _discountPolicy.ApplyDiscount(subTotal, 0, 10m); // adjust as needed
        subTotal -= discountAmount;

        var vatAmount = _vatPolicy.CalculateVAT(subTotal, 15m); // adjust as needed
        subTotal += vatAmount;

        // 4. Update sale totals and save
        sale.TotalAmount = subTotal;
        sale.DueAmount = sale.TotalAmount - sale.PaidAmount;

        await _salesRepository.UpdateAsync(sale, cancellationToken);

        // (Optional) Add a small delay for demonstration if you wish, otherwise remove:
        // await Task.Delay(3000);
    }

}

class CreateSaleTransactionCommandValidator : AbstractValidator<CreateSaleTransactionCommand>
{
    public CreateSaleTransactionCommandValidator()
    {
        RuleFor(x => x.SaleDate).NotEmpty().WithMessage("Sale date is required.");
        RuleFor(x => x.CustomerId).NotNull().WithMessage("Customer ID is required.");
        RuleFor(x => x.TotalAmount).GreaterThan(0).WithMessage("Total amount must be greater than zero.");
        RuleFor(x => x.PaidAmount).GreaterThanOrEqualTo(0).WithMessage("Paid amount cannot be negative.");
        RuleFor(x => x.DueAmount).GreaterThanOrEqualTo(0).WithMessage("Due amount cannot be negative.");
        RuleFor(x => x.SaleDetails).NotEmpty().WithMessage("At least one sale detail is required.");
        RuleForEach(x => x.SaleDetails).SetValidator(new SaleDetailDtoValidator());
    }
}

class SaleDetailDtoValidator : AbstractValidator<SaleDetailDto>
{
    public SaleDetailDtoValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("Product ID must be greater than zero.");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");
    }
}