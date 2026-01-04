using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetStuff.Order.Application.Features.Orders.Commands;
using PetStuff.Order.Application.Features.Orders.Queries;
using PetStuff.Order.Domain.Entities;
using System.Security.Claims;

namespace PetStuff.Order.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                   User.FindFirstValue("sub") ?? 
                   throw new UnauthorizedAccessException("User ID not found in token.");
        }

        // GET: api/orders
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var userId = GetUserId();
            var orders = await _mediator.Send(new GetOrdersByUserIdQuery { UserId = userId });
            return Ok(orders);
        }

        // GET: api/orders/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var userId = GetUserId();
            var order = await _mediator.Send(new GetOrderByIdQuery { OrderId = id, UserId = userId });
            
            if (order == null)
                return NotFound("Order not found.");
            
            return Ok(order);
        }

        // POST: api/orders
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {
            try
            {
                // UserId ve Items token'dan ve Basket Service'den otomatik alınacak
                var order = await _mediator.Send(command);
                return Ok(new { 
                    success = true, 
                    message = "Siparişiniz alındı. Admin onayından sonra kargoya verilecektir.",
                    order = order 
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
        }

        // PUT: api/orders/{id}/status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusCommand command)
        {
            command.OrderId = id;
            try
            {
                var result = await _mediator.Send(command);
                
                if (!result)
                    return NotFound("Order not found.");
                
                return Ok("Order status updated successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/orders/all (Admin only)
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var allOrders = await _mediator.Send(new GetAllOrdersQuery());
            return Ok(allOrders);
        }
    }
}

