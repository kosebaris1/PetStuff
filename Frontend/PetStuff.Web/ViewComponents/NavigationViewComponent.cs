using Microsoft.AspNetCore.Mvc;
using PetStuff.Web.Helpers;
using PetStuff.Web.Services;

namespace PetStuff.Web.ViewComponents
{
    public class NavigationViewComponent : ViewComponent
    {
        private readonly ICatalogService _catalogService;

        public NavigationViewComponent(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            if (string.IsNullOrEmpty(token))
            {
                return View(new NavigationViewModel
                {
                    Categories = new List<Models.CategoryViewModel>(),
                    Brands = new List<Models.BrandViewModel>()
                });
            }

            var categories = await _catalogService.GetCategoriesAsync(token);
            var brands = await _catalogService.GetBrandsAsync(token);

            return View(new NavigationViewModel
            {
                Categories = categories ?? new List<Models.CategoryViewModel>(),
                Brands = brands ?? new List<Models.BrandViewModel>()
            });
        }
    }

    public class NavigationViewModel
    {
        public List<Models.CategoryViewModel> Categories { get; set; } = new();
        public List<Models.BrandViewModel> Brands { get; set; } = new();
    }
}
