using InventoryService.Application.Dtos;
using InventoryService.Domain.Interfaces;
using MediatR;

namespace InventoryService.Application.Sales;
public class GetSalesSummaryQuery : IRequest<SalesSummaryDto>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class GetSalesSummaryQueryHandler : IRequestHandler<GetSalesSummaryQuery, SalesSummaryDto>
{
    private readonly ISalesRepository _salesRepository;
    public GetSalesSummaryQueryHandler(ISalesRepository salesRepository)
    {
        _salesRepository = salesRepository;
    }
    public async Task<SalesSummaryDto> Handle(GetSalesSummaryQuery request, CancellationToken cancellationToken)
    {

        var sales = await _salesRepository.FindAsync(s => s.SaleDate >= request.StartDate && s.SaleDate <= request.EndDate, 0, 100, cancellationToken);

        var totalSales = sales.Item1.Sum(s => s.TotalAmount);
        var totalRevenue = sales.Item1.Sum(s => s.PaidAmount);
        var numberOfTransactions = sales.Item1.Count();

        return new SalesSummaryDto
        {
            TotalSales = totalSales,
            TotalRevenue = totalRevenue,
            NumberOfTransactions = numberOfTransactions
        };
    }
}