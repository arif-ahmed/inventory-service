using InventoryService.Api.RequestModels;
using InventoryService.Application.Dtos;
using InventoryService.Application.Products;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Api.Controllers;

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
    public async Task<IActionResult> GetProducts()
    {
        _logger.LogInformation("Fetching list of products");
        var products = await _mediator.Send(new GetProductsQuery());
        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequestModel product)
    {

        if (product == null)
        {
            return BadRequest("Product data is required.");
        }

        await _mediator.Send(new CreateProductCommand { Name = product.Name, Barcode = product.Barcode, Category = product.Category, Price = product.Price, StockQty = product.StockQty });

        // return CreatedAtAction(nameof(GetProductById), new { id = 1 }, product);
        return Ok();
    }

    [HttpPut("{id}")]
    public IActionResult UpdateProduct(int id, [FromBody] ProductUpdateDto product)
    {
        _logger.LogInformation($"Updating product with ID: {id}");
        _mediator.Send(new UpdateProductCommand
        {
            ProductId = id,
            Name = product.Name,
            Barcode = product.Barcode,
            Category = product.Category,
            Price = product.Price,
            StockQty = product.StockQty
        });
        return Ok(new { Message = $"Product with ID: {id} updated successfully" });
    }

    [HttpDelete("{id}")]
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
