using InventoryService.Api.RequestModels;
using InventoryService.Application.Dtos;
using InventoryService.Application.Products;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryService.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IMediator _mediator;
    public ProductsController(ILogger<ProductsController> logger, IMediator mediator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces("application/json")]
    [SwaggerOperation(
        Summary = "Get Product by ID",
        Description = "Retrieves a product by its unique identifier.",
        OperationId = "GetProductById",
        Tags = new[] { "Products" }
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Product found", typeof(ProductDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid product ID")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while processing the request")]
    public async Task<IActionResult> GetProductById(int id)
    {
        _logger.LogInformation($"Fetching product details for ID: {id}");

        if (id <= 0)
        {
            return BadRequest("Invalid product ID.");
        }        

        var product = await _mediator.Send(new GetProductByIdQuery(id));
        return product != null ? Ok(product) : NotFound($"Product with ID: {id} not found.");
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces("application/json")]
    [SwaggerOperation(
        Summary = "Get Products",
        Description = "Retrieves a list of products with optional search and pagination.",
        OperationId = "GetProducts",
        Tags = new[] { "Products" }
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Products retrieved successfully", typeof(IEnumerable<ProductDto>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid search parameters")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while processing the request")]
    public async Task<IActionResult> GetProducts([FromQuery] string searchText, [FromQuery] int offset = 1, [FromQuery] int page = 10)
    {
        _logger.LogInformation("Fetching list of products");
        var products = await _mediator.Send(new GetProductsQuery { SearchTerm = searchText, PageNumber = offset, PageSize = page });
        return Ok(products);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces("application/json")]
    [SwaggerOperation(
        Summary = "Create Product",
        Description = "Creates a new product in the inventory.",
        OperationId = "CreateProduct",
        Tags = new[] { "Products" }
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Product created successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid product data")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while processing the request")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        _logger.LogInformation($"Creating product with details: {command}");        
        if (command == null)
        {
            return BadRequest("Product data is required.");
        }

        await _mediator.Send(command);
        _logger.LogInformation("Product created successfully");
        return Ok(new { Message = "Product created successfully" });
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces("application/json")]
    [SwaggerOperation(
        Summary = "Update Product",
        Description = "Updates an existing product in the inventory.",
        OperationId = "UpdateProduct",
        Tags = new[] { "Products" }
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Product updated successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid product data or ID")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while processing the request")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductCommand command)
    {
        _logger.LogInformation($"Updating product with ID: {id}");
        await _mediator.Send(command);
        _logger.LogInformation($"Product with ID: {id} updated successfully");
        return Ok(new { Message = $"Product with ID: {id} updated successfully" });
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces("application/json")]
    [SwaggerOperation(
        Summary = "Delete Product",
        Description = "Deletes a product from the inventory by its ID.",
        OperationId = "DeleteProduct",
        Tags = new[] { "Products" }
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Product deleted successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid product ID")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while processing the request")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        // This method would typically call a service to delete an existing product
        if (id <= 0)
        {
            return BadRequest("Invalid product ID.");
        }

        _logger.LogInformation($"Deleting product with ID: {id}");

        await _mediator.Send(new DeleteProductCommand { ProductId = id });

        return Ok(new { Message = $"Product with ID: {id} deleted successfully" });
    }
}
