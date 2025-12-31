using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetStuff.Web.Helpers;
using PetStuff.Web.Models;
using PetStuff.Web.Services;

namespace PetStuff.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ICatalogService _catalogService;

        public AdminController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        public async Task<IActionResult> Index()
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Please login to access admin panel.";
                return RedirectToAction("Login", "Account");
            }

            var products = await _catalogService.GetProductsAsync(token);
            var categories = await _catalogService.GetCategoriesAsync(token);
            var brands = await _catalogService.GetBrandsAsync(token);

            // Debug: Check if data is coming from API
            if (products == null || products.Count == 0)
            {
                TempData["WarningMessage"] = "No products found. Please check if Catalog API is running and accessible.";
            }
            
            if (categories == null || categories.Count == 0)
            {
                TempData["WarningMessage"] = "No categories found. Please check if Catalog API is running and accessible.";
            }

            ViewBag.Categories = categories ?? new List<CategoryViewModel>();
            ViewBag.Brands = brands ?? new List<BrandViewModel>();

            return View(products ?? new List<ProductListViewModel>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(CreateProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill in all required fields.";
                return RedirectToAction("Index");
            }

            var token = SessionHelper.GetToken(HttpContext.Session);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            // Parse ImageUrls from textarea
            if (!string.IsNullOrEmpty(Request.Form["ImageUrls"]))
            {
                var imageUrlsText = Request.Form["ImageUrls"].ToString();
                model.ImageUrls = imageUrlsText
                    .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(url => !string.IsNullOrWhiteSpace(url))
                    .Select(url => url.Trim())
                    .ToList();
            }

            var success = await _catalogService.CreateProductAsync(model, token);

            if (success)
            {
                TempData["SuccessMessage"] = "Product created successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to create product. Please try again.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill in all required fields.";
                return RedirectToAction("Index");
            }

            var token = SessionHelper.GetToken(HttpContext.Session);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            // Parse ImageUrls from textarea
            if (!string.IsNullOrEmpty(Request.Form["ImageUrls"]))
            {
                var imageUrlsText = Request.Form["ImageUrls"].ToString();
                model.ImageUrls = imageUrlsText
                    .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(url => !string.IsNullOrWhiteSpace(url))
                    .Select(url => url.Trim())
                    .ToList();
            }

            var success = await _catalogService.UpdateProductAsync(id, model, token);

            if (success)
            {
                TempData["SuccessMessage"] = "Product updated successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update product. Please try again.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var success = await _catalogService.DeleteProductAsync(id, token);

            if (success)
            {
                TempData["SuccessMessage"] = "Product deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete product. Please try again.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct(int id)
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            if (string.IsNullOrEmpty(token))
            {
                return Json(new { success = false });
            }

            var product = await _catalogService.GetProductByIdAsync(id, token);
            if (product == null)
            {
                return Json(new { success = false });
            }

            return Json(new
            {
                success = true,
                product = new
                {
                    id = product.Id,
                    name = product.Name,
                    description = product.Description,
                    price = product.Price,
                    stock = product.Stock,
                    isActive = product.IsActive,
                    categoryId = product.Category?.Id ?? 0,
                    brandId = product.Brand?.Id ?? 0,
                    imageUrls = product.ImageUrls != null ? string.Join("\n", product.ImageUrls) : ""
                }
            });
        }
    }
}

