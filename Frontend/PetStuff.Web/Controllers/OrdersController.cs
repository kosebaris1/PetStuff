using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetStuff.Web.Helpers;
using PetStuff.Web.Models;
using PetStuff.Web.Services;

namespace PetStuff.Web.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IBasketService _basketService;

        public OrdersController(IOrderService orderService, IBasketService basketService)
        {
            _orderService = orderService;
            _basketService = basketService;
        }

        public async Task<IActionResult> Index()
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = await _orderService.GetOrdersAsync(token);
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var order = await _orderService.GetOrderByIdAsync(id, token);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var basket = await _basketService.GetBasketAsync(token);
            if (basket == null || basket.Items == null || !basket.Items.Any())
            {
                TempData["ErrorMessage"] = "Your basket is empty. Add items to basket before placing an order.";
                return RedirectToAction("Index", "Basket");
            }

            return View(new CreateOrderViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var token = SessionHelper.GetToken(HttpContext.Session);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var order = await _orderService.CreateOrderAsync(model, token);

            if (order == null)
            {
                ModelState.AddModelError("", "Sipariş oluşturulamadı. Lütfen tekrar deneyin.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Siparişiniz alındı! Admin onayından sonra kargoya verilecektir.";
            return RedirectToAction("Details", new { id = order.Id });
        }
    }
}

