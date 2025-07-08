using InventoryService.Application.Sales;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class SalesReportController : ControllerBase
{
    private readonly IMediator _mediator;
    public SalesReportController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet]
    public async Task<IActionResult> GetSalesSummary([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
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

        return Ok(salesSummary);
    }
}
