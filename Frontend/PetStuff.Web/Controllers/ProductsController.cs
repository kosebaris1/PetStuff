using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetStuff.Web.Helpers;
using PetStuff.Web.Models;
using PetStuff.Web.Services;

namespace PetStuff.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ICatalogService _catalogService;

        public ProductsController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        // Public - ziyaretçiler de görebilir
        public async Task<IActionResult> Index()
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            
            // Token olsa da olmasa da ürünleri çek (public endpoint)
            var products = await _catalogService.GetProductsAsync(token);
            return View(products);
        }

        // Public - ziyaretçiler de görebilir
        public async Task<IActionResult> Details(int id)
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            
            // Token olsa da olmasa da ürün detaylarını çek (public endpoint)
            var product = await _catalogService.GetProductByIdAsync(id, token);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // Public - ziyaretçiler de görebilir
        public async Task<IActionResult> Category(int categoryId)
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            var products = await _catalogService.GetProductsByCategoryAsync(categoryId, token);
            return View("Index", products);
        }

        // Public - ziyaretçiler de görebilir
        public async Task<IActionResult> Brand(int brandId)
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            var products = await _catalogService.GetProductsByBrandAsync(brandId, token);
            return View("Index", products);
        }

        // Public - ziyaretçiler de görebilir
        public async Task<IActionResult> Categories()
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            var categories = await _catalogService.GetCategoriesAsync(token);
            
            // Her kategori için ürün sayısını hesapla
            foreach (var category in categories)
            {
                var products = await _catalogService.GetProductsByCategoryAsync(category.Id, token);
                category.ProductCount = products.Count;
            }
            
            ViewBag.Categories = categories;
            return View(categories);
        }

        // Public - ziyaretçiler de görebilir
        public async Task<IActionResult> Brands()
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            var brands = await _catalogService.GetBrandsAsync(token);
            
            // Her marka için ürün sayısını hesapla
            foreach (var brand in brands)
            {
                var products = await _catalogService.GetProductsByBrandAsync(brand.Id, token);
                brand.ProductCount = products.Count;
            }
            
            ViewBag.Brands = brands;
            return View(brands);
        }
    }
}

