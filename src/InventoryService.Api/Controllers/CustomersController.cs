using InventoryService.Application.Customers;
using InventoryService.Application.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryService.Api.Controllers;

// [Authorize]
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
    [SwaggerOperation(Summary = "Get all customers", Description = "Fetches a list of all customers.")]
    [SwaggerResponse(200, "List of customers", typeof(IEnumerable<CustomerDto>))]
    [SwaggerResponse(404, "No customers found")]
    public async Task<IActionResult> GetCustomers()
    {
        _logger.LogInformation("Fetching all customers");
        var customers = await _mediator.Send(new GetCustomersQuery());
        return Ok(customers);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new customer", Description = "Creates a new customer in the system.")]
    [SwaggerResponse(200, "Customer created successfully")]
    [SwaggerResponse(400, "Invalid customer data")]
    [SwaggerResponse(500, "Internal server error")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(string), 400)]
    [ProducesResponseType(typeof(string), 500)]
    public async Task<IActionResult> Post(CreateCustomerCommand command)
    {
        _logger.LogInformation($"Create customer request: {command}");
        await _mediator.Send(command);
        _logger.LogInformation("Customer created successfully");
        return Ok("Customer created successfully");
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Update an existing customer", Description = "Updates the details of an existing customer.")]
    [SwaggerResponse(200, "Customer updated successfully")]
    [SwaggerResponse(400, "Invalid customer data")]
    [SwaggerResponse(404, "Customer not found")]
    [SwaggerResponse(500, "Internal server error")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(string), 400)]
    [ProducesResponseType(typeof(string), 404)]
    [ProducesResponseType(typeof(string), 500)]    
    public async Task<IActionResult> Put(int id, [FromBody] UpdateCustomerCommand customer)
    {
        _logger.LogInformation("Updating customer with ID: {CustomerId}", id);

        if (id <= 0)
        {
            _logger.LogError("Invalid customer ID: {CustomerId}", id);
            return BadRequest("Invalid customer ID.");
        }

        if (customer == null)
        {
            _logger.LogError("Customer data is null for ID: {CustomerId}", id);
            return BadRequest("Customer data cannot be null.");
        }

        if (id != customer.CustomerId)
        {
            _logger.LogError("Customer ID mismatch: provided {ProvidedId}, expected {ExpectedId}", customer.CustomerId, id);
            return BadRequest("Customer ID mismatch.");
        }

        _logger.LogInformation("Updating customer with ID: {CustomerId} with data: {@CustomerData}", id, customer);

        customer.CustomerId = id;
        var status = await _mediator.Send(new UpdateCustomerCommand { CustomerId = id, FullName = customer.FullName, Email = customer.Email, Phone = customer.Phone, });        
        return Ok(status);
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Delete a customer", Description = "Deletes a customer from the system by their unique identifier.")]
    [SwaggerResponse(200, "Customer deleted successfully")]
    [SwaggerResponse(404, "Customer not found")]
    [SwaggerResponse(500, "Internal server error")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(string), 404)]
    [ProducesResponseType(typeof(string), 500)]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting customer with ID: {CustomerId}", id);
        await _mediator.Send(new DeleteCustomerCommand { CustomerId = id });
        _logger.LogInformation("Customer with ID: {CustomerId} deleted successfully", id);
        return Ok("Customer deleted successfully");
    }
}
