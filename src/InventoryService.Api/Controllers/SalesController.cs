using InventoryService.Api.Filters;
using InventoryService.Application.Sales;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryService.Api.Controllers;

[Authorize]
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
    [SwaggerOperation(Summary = "Create a new sale transaction", Description = "Creates a new sale transaction with details.")]
    [SwaggerResponse(200, "Sale transaction created successfully")]
    [SwaggerResponse(400, "Invalid sale data")]
    [SwaggerResponse(500, "Internal server error")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(string), 400)]
    [ProducesResponseType(typeof(string), 500)]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleTransactionCommand command)
    {
        _logger.LogInformation("Creating a new sale transaction");
        await _mediator.Send(command);
        _logger.LogInformation("Sale transaction created successfully");
        return Ok("Sale transaction created successfully");
    }
}
