using InventoryService.Api.Filters;
using InventoryService.Api.RequestModels;
using InventoryService.Application.Sales;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Api.Controllers;

// [Authorize]
[Route("api/[controller]")]
[ApiController]
public class SalesController : ControllerBase
{
    private readonly ILogger<SalesController> _logger;
    private readonly IMediator _mediator;
    public SalesController(ILogger<SalesController> logger, IMediator mediator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [ServiceFilter(typeof(SalesConcurrencyFilter))]
    [HttpPost]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequestModel request)
    {
        _logger.LogInformation("Creating a new sale transaction");

        await _mediator.Send(new CreateSaleTransactionCommand
        {
            SaleDate = request.SaleDate,
            CustomerId = request.CustomerId,
            TotalAmount = request.TotalAmount,
            PaidAmount = request.PaidAmount,
            DueAmount = request.DueAmount,
            SaleDetails = request.SaleDetails.Select(sd => new Application.Sales.SaleDetailDto
            {
                ProductId = sd.ProductId,
                Quantity = sd.Quantity,
                Price = sd.Price
            }).ToList()
        });

        return Ok();
    }
}
