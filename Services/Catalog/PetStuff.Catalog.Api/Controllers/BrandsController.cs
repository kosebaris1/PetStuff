using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetStuff.Catalog.Application.Features.Brands.Commands;
using PetStuff.Catalog.Application.Features.Brands.Queries;

namespace PetStuff.Catalog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BrandsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/brands - Public endpoint for visitors
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var brands = await _mediator.Send(new GetAllBrandsQuery());
            return Ok(brands);
        }

        // GET: api/brands/{id} - Public endpoint for visitors
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var brand = await _mediator.Send(new GetBrandByIdQuery(id));
            if (brand == null)
                return NotFound("Brand not found.");
            return Ok(brand);
        }

        // POST: api/brands
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateBrandCommand command)
        {
            await _mediator.Send(command);
            return Ok("Brand created successfully.");
        }

        // PUT: api/brands/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBrandCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound("Brand not found.");

            return Ok("Brand updated successfully.");
        }

        // DELETE: api/brands/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new RemoveBrandCommand(id));

            if (!result)
                return NotFound("Brand not found.");

            return Ok("Brand deleted successfully.");
        }
    }
}
