using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetStuff.Catalog.Application.Features.Products.Commands;
using PetStuff.Catalog.Application.Features.Products.Queries;
using OrderItemStockRequest = PetStuff.Catalog.Application.Features.Products.Commands.OrderItemStockRequest;

namespace PetStuff.Catalog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/products - Public endpoint for visitors
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var products = await _mediator.Send(new GetAllProductsQuery());
            return Ok(products);
        }

        // GET: api/products/{id} - Public endpoint for visitors
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _mediator.Send(new GetProductByIdQuery(id));
            if (product == null)
                return NotFound("Product not found.");
            return Ok(product);
        }

        // GET: api/products/category/{categoryId} - Public endpoint for visitors
        [HttpGet("category/{categoryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var products = await _mediator.Send(new GetProductsByCategoryQuery(categoryId));
            return Ok(products);
        }

        // GET: api/products/brand/{brandId} - Public endpoint for visitors
        [HttpGet("brand/{brandId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByBrand(int brandId)
        {
            var products = await _mediator.Send(new GetProductsByBrandQuery(brandId));
            return Ok(products);
        }

        // POST: api/products
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
        {
            await _mediator.Send(command);
            return Ok("Product created successfully.");
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductCommand command)
        {
            command.Id = id;

            var result = await _mediator.Send(command);

            if (!result)
                return NotFound("Product not found.");

            return Ok("Product updated successfully.");
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new RemoveProductCommand(id));

            if (!result)
                return NotFound("Product not found.");

            return Ok("Product deleted successfully.");
        }

        // POST: api/products/{id}/check-stock
        [HttpPost("{id}/check-stock")]
        [Authorize]
        public async Task<IActionResult> CheckStock(int id, [FromBody] CheckStockRequest request)
        {
            var command = new CheckStockCommand
            {
                ProductId = id,
                Quantity = request.Quantity
            };

            var isAvailable = await _mediator.Send(command);
            
            return Ok(new { isAvailable, productId = id, requestedQuantity = request.Quantity });
        }

        // POST: api/products/reduce-stock-for-order
        [HttpPost("reduce-stock-for-order")]
        [Authorize]
        public async Task<IActionResult> ReduceStockForOrder([FromBody] List<OrderItemStockRequest> items)
        {
            var command = new ReduceStockForOrderCommand
            {
                Items = items
            };

            var success = await _mediator.Send(command);
            
            if (!success)
            {
                return BadRequest("Insufficient stock for one or more products.");
            }

            return Ok("Stock reduced successfully.");
        }
    }

    public class CheckStockRequest
    {
        public int Quantity { get; set; }
    }
}
