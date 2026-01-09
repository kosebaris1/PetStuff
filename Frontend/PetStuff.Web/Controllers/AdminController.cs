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
        private readonly IOrderService _orderService;
        private readonly IFileUploadService _fileUploadService;

        public AdminController(ICatalogService catalogService, IOrderService orderService, IFileUploadService fileUploadService)
        {
            _catalogService = catalogService;
            _orderService = orderService;
            _fileUploadService = fileUploadService;
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

            // Calculate product counts for categories and brands
            var categoriesWithCount = (categories ?? new List<CategoryViewModel>()).Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                CreatedDate = c.CreatedDate,
                UpdatedDate = c.UpdatedDate,
                ProductCount = products?.Count(p => p.CategoryName == c.Name) ?? 0
            }).ToList();

            var brandsWithCount = (brands ?? new List<BrandViewModel>()).Select(b => new BrandViewModel
            {
                Id = b.Id,
                Name = b.Name,
                CreatedDate = b.CreatedDate,
                UpdatedDate = b.UpdatedDate,
                ProductCount = products?.Count(p => p.BrandName == b.Name) ?? 0
            }).ToList();

            ViewBag.Categories = categoriesWithCount;
            ViewBag.Brands = brandsWithCount;
            ViewBag.Products = products ?? new List<ProductListViewModel>();
            ViewBag.TotalProducts = products?.Count ?? 0;
            ViewBag.TotalCategories = categoriesWithCount.Count;
            ViewBag.TotalBrands = brandsWithCount.Count;

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

            // Upload product images from files
            var imageUrls = new List<string>();
            
            // File upload'dan gelen resimleri yükle
            if (Request.Form.Files != null && Request.Form.Files.Count > 0)
            {
                var uploadedUrls = await _fileUploadService.UploadProductImagesAsync(Request.Form.Files);
                imageUrls.AddRange(uploadedUrls);
            }

            // Textarea'dan gelen URL'leri de ekle (eski yöntem - geriye dönük uyumluluk için)
            if (!string.IsNullOrEmpty(Request.Form["ImageUrls"]))
            {
                var imageUrlsText = Request.Form["ImageUrls"].ToString();
                var textUrls = imageUrlsText
                    .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(url => !string.IsNullOrWhiteSpace(url))
                    .Select(url => url.Trim())
                    .ToList();
                imageUrls.AddRange(textUrls);
            }

            model.ImageUrls = imageUrls;

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

            // Mevcut ürünü çek (mevcut image URL'lerini korumak için)
            var existingProduct = await _catalogService.GetProductByIdAsync(id, token);
            var imageUrls = new List<string>();

            // Önce mevcut image'ları ekle (korunacak)
            if (existingProduct?.ImageUrls != null && existingProduct.ImageUrls.Any())
            {
                imageUrls.AddRange(existingProduct.ImageUrls);
            }

            // Textarea'dan gelen URL'leri kontrol et
            var hasTextareaUrls = !string.IsNullOrEmpty(Request.Form["ImageUrls"]);
            var textareaUrls = new List<string>();
            
            if (hasTextareaUrls)
            {
                var imageUrlsText = Request.Form["ImageUrls"].ToString();
                textareaUrls = imageUrlsText
                    .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(url => !string.IsNullOrWhiteSpace(url))
                    .Select(url => url.Trim())
                    .ToList();
            }

            // Eğer textarea'da URL'ler varsa, mevcut image'ları temizle ve textarea'dakileri kullan
            // (Kullanıcı image'ları değiştirmek istiyor demektir)
            if (hasTextareaUrls && textareaUrls.Any())
            {
                imageUrls.Clear();
                imageUrls.AddRange(textareaUrls);
            }

            // File upload'dan gelen yeni resimleri ekle (her zaman eklenir, mevcut image'lara ek olarak)
            if (Request.Form.Files != null && Request.Form.Files.Count > 0)
            {
                var uploadedUrls = await _fileUploadService.UploadProductImagesAsync(Request.Form.Files);
                if (uploadedUrls.Any())
                {
                    // Eğer textarea boşsa, yeni dosyaları mevcut image'lara ekle
                    // Eğer textarea doluysa, yeni dosyaları textarea'daki URL'lere ekle
                    imageUrls.AddRange(uploadedUrls);
                }
            }

            model.ImageUrls = imageUrls;

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

        // Category CRUD
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CreateCategoryViewModel model)
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

            var success = await _catalogService.CreateCategoryAsync(model, token);
            if (success)
            {
                TempData["SuccessMessage"] = "Category created successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to create category. Please try again.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryViewModel model)
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

            var success = await _catalogService.UpdateCategoryAsync(id, model, token);
            if (success)
            {
                TempData["SuccessMessage"] = "Category updated successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update category. Please try again.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var success = await _catalogService.DeleteCategoryAsync(id, token);
            if (success)
            {
                TempData["SuccessMessage"] = "Category deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete category. Please try again.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetCategory(int id)
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            if (string.IsNullOrEmpty(token))
            {
                return Json(new { success = false });
            }

            var category = await _catalogService.GetCategoryByIdAsync(id, token);
            if (category == null)
            {
                return Json(new { success = false });
            }

            return Json(new
            {
                success = true,
                category = new
                {
                    id = category.Id,
                    name = category.Name,
                    description = category.Description
                }
            });
        }

        // Brand CRUD
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBrand(CreateBrandViewModel model)
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

            var success = await _catalogService.CreateBrandAsync(model, token);
            if (success)
            {
                TempData["SuccessMessage"] = "Brand created successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to create brand. Please try again.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBrand(int id, UpdateBrandViewModel model)
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

            var success = await _catalogService.UpdateBrandAsync(id, model, token);
            if (success)
            {
                TempData["SuccessMessage"] = "Brand updated successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update brand. Please try again.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var success = await _catalogService.DeleteBrandAsync(id, token);
            if (success)
            {
                TempData["SuccessMessage"] = "Brand deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete brand. Please try again.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetBrand(int id)
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            if (string.IsNullOrEmpty(token))
            {
                return Json(new { success = false });
            }

            var brand = await _catalogService.GetBrandByIdAsync(id, token);
            if (brand == null)
            {
                return Json(new { success = false });
            }

            return Json(new
            {
                success = true,
                brand = new
                {
                    id = brand.Id,
                    name = brand.Name
                }
            });
        }

        // Orders Management
        public async Task<IActionResult> Orders()
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Please login to access admin panel.";
                return RedirectToAction("Login", "Account");
            }

            var orders = await _orderService.GetAllOrdersAsync(token);
            return View(orders ?? new List<OrderViewModel>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int id, string status)
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var success = await _orderService.UpdateOrderStatusAsync(id, status, token);
            
            if (success)
            {
                if (status == "Confirmed")
                {
                    TempData["SuccessMessage"] = "Sipariş onaylandı. Kullanıcıya bildirim gönderilecek.";
                }
                else if (status == "Cancelled")
                {
                    TempData["SuccessMessage"] = "Sipariş iptal edildi.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Sipariş durumu güncellendi.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Sipariş durumu güncellenemedi. Lütfen tekrar deneyin.";
            }

            return RedirectToAction("Orders");
        }
    }
}

