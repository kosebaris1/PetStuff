using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetStuff.Web.Helpers;
using PetStuff.Web.Models;
using PetStuff.Web.Services;

namespace PetStuff.Web.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ICatalogService _catalogService;

        public ProductsController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        public async Task<IActionResult> Index()
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var products = await _catalogService.GetProductsAsync(token);
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var product = await _catalogService.GetProductByIdAsync(id, token);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public async Task<IActionResult> Category(int categoryId)
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var products = await _catalogService.GetProductsByCategoryAsync(categoryId, token);
            return View("Index", products);
        }

        public async Task<IActionResult> Brand(int brandId)
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var products = await _catalogService.GetProductsByBrandAsync(brandId, token);
            return View("Index", products);
        }

        public async Task<IActionResult> Categories()
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var categories = await _catalogService.GetCategoriesAsync(token);
            ViewBag.Categories = categories;
            return View(categories);
        }

        public async Task<IActionResult> Brands()
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var brands = await _catalogService.GetBrandsAsync(token);
            ViewBag.Brands = brands;
            return View(brands);
        }
    }
}

