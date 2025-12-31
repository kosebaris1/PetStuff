using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PetStuff.Web.Helpers;
using PetStuff.Web.Models;
using PetStuff.Web.Services;

namespace PetStuff.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IIdentityService _identityService;

        public AccountController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var token = await _identityService.LoginAsync(model.Username, model.Password);

            if (string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }

            // Token'ı hem session'a hem de TempData'ya kaydet (localStorage için JavaScript'e iletmek üzere)
            SessionHelper.SetToken(HttpContext.Session, token);
            TempData["AccessToken"] = token; // JavaScript ile localStorage'a kaydedilmek üzere

            // Token'dan claims çıkar (role bilgisi dahil)
            var tokenClaims = JwtHelper.ParseToken(token);
            var claims = new List<System.Security.Claims.Claim>(tokenClaims);

            // Eğer claim yoksa username ekle
            if (!claims.Any(c => c.Type == System.Security.Claims.ClaimTypes.Name))
            {
                claims.Add(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, model.Username));
            }

            var claimsIdentity = new System.Security.Claims.ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new System.Security.Claims.ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Role'e göre yönlendirme
            var userRole = claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Admin ise Admin sayfasına, User ise Products sayfasına yönlendir
            if (userRole == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "Products");
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = await _identityService.RegisterAsync(
                model.Username, 
                model.Email, 
                model.Password, 
                model.FullName);

            if (!success)
            {
                ModelState.AddModelError("", "Registration failed. Please try again.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Registration successful! Please login.";
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            SessionHelper.RemoveToken(HttpContext.Session);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["ClearLocalStorage"] = "true"; // JavaScript ile localStorage'ı temizlemek için
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

