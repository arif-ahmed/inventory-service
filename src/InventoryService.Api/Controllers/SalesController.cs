using InventoryService.Api.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SalesController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateSale([FromBody] CreateSaleRequestModel request)
    {
        // Logic to create a sale
        return Ok();
    }
}
