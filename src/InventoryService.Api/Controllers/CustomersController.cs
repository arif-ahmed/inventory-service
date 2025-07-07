using InventoryService.Application.Customers;
using InventoryService.Domain.Entities.Products;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly ILogger<CustomersController> _logger;
    private readonly IMediator _mediator;

    public CustomersController(ILogger<CustomersController> logger, IMediator mediator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get a product by ID", Description = "Returns a single product.")]
    [SwaggerResponse(200, "Product found", typeof(Product))]
    [SwaggerResponse(404, "Product not found")]
    public async Task<IActionResult> GetCustomerById(string id)
    {
        _logger.LogInformation("Fetching customer with ID: {CustomerId}", id);
        var customer = await _mediator.Send(new GetCustomerByIdQuery(id));
        if (customer == null)
        {
            _logger.LogWarning("Customer with ID: {CustomerId} not found", id);
            return NotFound();
        }
        return Ok(customer);
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers()
    {
        _logger.LogInformation("Fetching all customers");
        var customers = await _mediator.Send(new GetCustomersQuery());
        return Ok(customers);
    }
}
