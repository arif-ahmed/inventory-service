using InventoryService.Application.Dtos;
using InventoryService.Application.Sales;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryService.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class SalesReportController : ControllerBase
{
    private readonly ILogger<SalesReportController> _logger;
    private readonly IMediator _mediator;
    public SalesReportController(ILogger<SalesReportController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet]
    [Route("summary")]
    [ProducesResponseType(typeof(SalesSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Get Sales Summary",
        Description = "Retrieves a summary of sales within a specified date range.",
        OperationId = "GetSalesSummary"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns the sales summary for the specified date range.", typeof(SalesSummaryDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid date range provided.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized to access this resource.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Sales summary not found for the specified date range.")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.")]
    public async Task<IActionResult> GetSalesSummary([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        _logger.LogInformation("GetSalesSummary called with startDate: {StartDate}, endDate: {EndDate}", startDate, endDate);

        if (startDate == default || endDate == default)
        {
            return BadRequest("Start date and end date must be provided.");
        }

        if (startDate > endDate)
        {
            return BadRequest("Start date cannot be later than end date.");
        }

        var query = new GetSalesSummaryQuery
        {
            StartDate = startDate,
            EndDate = endDate
        };

        var salesSummary = await _mediator.Send(query);

        if (salesSummary == null)
        {
            _logger.LogWarning("No sales summary found for the specified date range: {StartDate} to {EndDate}", startDate, endDate);
            return NotFound("Sales summary not found for the specified date range.");
        }

        _logger.LogInformation("Sales summary retrieved successfully for the date range: {StartDate} to {EndDate}", startDate, endDate);

        return Ok(salesSummary);
    }
}
