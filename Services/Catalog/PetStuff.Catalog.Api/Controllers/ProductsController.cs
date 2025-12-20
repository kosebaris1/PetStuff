using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetStuff.Catalog.Application.Features.Products.Commands;
using PetStuff.Catalog.Application.Features.Products.Queries;

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

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _mediator.Send(new GetAllProductsQuery());
            return Ok(products);
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _mediator.Send(new GetProductByIdQuery(id));
            if (product == null)
                return NotFound("Product not found.");
            return Ok(product);
        }

        // GET: api/products/category/{categoryId}
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var products = await _mediator.Send(new GetProductsByCategoryQuery(categoryId));
            return Ok(products);
        }

        // GET: api/products/brand/{brandId}
        [HttpGet("brand/{brandId}")]
        public async Task<IActionResult> GetByBrand(int brandId)
        {
            var products = await _mediator.Send(new GetProductsByBrandQuery(brandId));
            return Ok(products);
        }

        // POST: api/products
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
        {
            await _mediator.Send(command);
            return Ok("Product created successfully.");
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
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
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new RemoveProductCommand(id));

            if (!result)
                return NotFound("Product not found.");

            return Ok("Product deleted successfully.");
        }
    }
}
