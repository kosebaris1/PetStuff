using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetStuff.Web.Helpers;
using PetStuff.Web.Models;
using PetStuff.Web.Services;

namespace PetStuff.Web.Controllers;

public class HomeController : Controller
{
    private readonly ICatalogService _catalogService;

    public HomeController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    public async Task<IActionResult> Index()
    {
        // If user is authenticated, show products
        if (User.Identity?.IsAuthenticated == true)
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                var token = SessionHelper.GetToken(HttpContext.Session);
                if (!string.IsNullOrEmpty(token))
                {
                    var products = await _catalogService.GetProductsAsync(token);
                    ViewBag.Products = products.Take(6).ToList(); // Show first 6 products
                }
            }
        }
        // Ziyaretçiler için boş liste göster (ürünleri görmek için login olmaları gerektiğini belirtmek için)
        return View();
    }

    [Authorize]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
