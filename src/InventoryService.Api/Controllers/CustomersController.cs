using InventoryService.Api.RequestModels;
using InventoryService.Application.Customers;
using InventoryService.Application.Dtos;
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
    [SwaggerOperation(Summary = "Get customer by ID", Description = "Fetches a customer by their unique identifier.")]
    [SwaggerResponse(200, "Customer found", typeof(CustomerDto))]
    [SwaggerResponse(404, "Customer not found")]
    public async Task<IActionResult> GetCustomerById(int id)
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

    [HttpPost]
    public async Task<IActionResult> Post(CustomerCreateRequestModel payload)
    {
        _logger.LogInformation($"Create customer request: {payload}");
        await _mediator.Send(new CreateCustomerCommand { FullName = payload.FullName, Email = payload.Email, Phone = payload.Phone });
        return Ok("Customer created successfully");
    }

    [HttpPut]
    public async Task<IActionResult> Put(int id, [FromBody] CustomerDto customer)
    {
        _logger.LogInformation($"Update customer request: {customer}");
        var status = await _mediator.Send(new UpdateCustomerCommand { CustomerId = id, FullName = customer.FullName, Email = customer.Email, Phone = customer.Phone, });
        return Ok(status);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting customer with ID: {CustomerId}", id);
        await _mediator.Send(new DeleteCustomerCommand { CustomerId = id });
        return Ok();
    }
}
