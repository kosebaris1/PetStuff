using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetStuff.Basket.Application.Features.Basket.Commands;
using PetStuff.Basket.Application.Features.Basket.Queries;
using System.Security.Claims;

namespace PetStuff.Basket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BasketController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BasketController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                   User.FindFirstValue("sub") ?? 
                   throw new UnauthorizedAccessException("User ID not found in token.");
        }

        // GET: api/basket
        [HttpGet]
        public async Task<IActionResult> GetBasket()
        {
            var userId = GetUserId();
            var basket = await _mediator.Send(new GetBasketQuery { UserId = userId });
            
            if (basket == null)
            {
                return Ok(new { UserId = userId, Items = new List<object>(), TotalPrice = 0 });
            }
            
            return Ok(basket);
        }

        // POST: api/basket/items
        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddItemToBasketCommand command)
        {
            command.UserId = GetUserId();
            var result = await _mediator.Send(command);
            
            if (!result)
                return BadRequest("Failed to add item to basket.");
            
            return Ok("Item added to basket successfully.");
        }

        // PUT: api/basket/items/{productId}/quantity
        [HttpPut("items/{productId}/quantity")]
        public async Task<IActionResult> UpdateItemQuantity(int productId, [FromBody] UpdateItemQuantityCommand command)
        {
            command.UserId = GetUserId();
            command.ProductId = productId;
            
            var result = await _mediator.Send(command);
            
            if (!result)
                return NotFound("Item not found in basket.");
            
            return Ok("Item quantity updated successfully.");
        }

        // DELETE: api/basket/items/{productId}
        [HttpDelete("items/{productId}")]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            var command = new RemoveItemFromBasketCommand
            {
                UserId = GetUserId(),
                ProductId = productId
            };
            
            var result = await _mediator.Send(command);
            
            if (!result)
                return NotFound("Item not found in basket.");
            
            return Ok("Item removed from basket successfully.");
        }

        // DELETE: api/basket
        [HttpDelete]
        public async Task<IActionResult> ClearBasket()
        {
            var command = new ClearBasketCommand
            {
                UserId = GetUserId()
            };
            
            var result = await _mediator.Send(command);
            
            if (!result)
                return NotFound("Basket not found.");
            
            return Ok("Basket cleared successfully.");
        }
    }
}

