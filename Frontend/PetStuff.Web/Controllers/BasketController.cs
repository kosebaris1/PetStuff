using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetStuff.Web.Helpers;
using PetStuff.Web.Models;
using PetStuff.Web.Services;

namespace PetStuff.Web.Controllers
{
    [Authorize]
    public class BasketController : Controller
    {
        private readonly IBasketService _basketService;
        private readonly ICatalogService _catalogService;

        public BasketController(IBasketService basketService, ICatalogService catalogService)
        {
            _basketService = basketService;
            _catalogService = catalogService;
        }

        public async Task<IActionResult> Index()
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var basket = await _basketService.GetBasketAsync(token);
            return View(basket ?? new BasketViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId, int quantity = 1)
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var product = await _catalogService.GetProductByIdAsync(productId, token);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToAction("Index", "Products");
            }

            var addToBasket = new AddToBasketViewModel
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = quantity,
                ProductImageUrl = product.ImageUrls?.FirstOrDefault()
            };

            var success = await _basketService.AddItemToBasketAsync(addToBasket, token);

            if (success)
            {
                TempData["SuccessMessage"] = "Product added to basket successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to add product to basket.";
            }

            return RedirectToAction("Index", "Products");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var success = await _basketService.UpdateItemQuantityAsync(productId, quantity, token);

            if (!success)
            {
                TempData["ErrorMessage"] = "Failed to update item quantity.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int productId)
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var success = await _basketService.RemoveItemFromBasketAsync(productId, token);

            if (success)
            {
                TempData["SuccessMessage"] = "Item removed from basket.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to remove item from basket.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var success = await _basketService.ClearBasketAsync(token);

            if (success)
            {
                TempData["SuccessMessage"] = "Basket cleared.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to clear basket.";
            }

            return RedirectToAction("Index");
        }
    }
}

