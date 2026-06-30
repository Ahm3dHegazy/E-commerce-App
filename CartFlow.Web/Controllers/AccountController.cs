using CartFlow.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace CartFlow.Web.Controllers {
    public class AccountController(IAccountService accountService) : Controller {
        public IActionResult SignIn() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(string email, string password, string? returnUrl) {
            var user = await accountService.SignInAsync(email, password);
            if (user is null) {
                ModelState.AddModelError("", "Invalid Email or Password.");
                return View();
            }

            var claims = new List<Claim> {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);

            return RedirectToLocal(returnUrl);
        }

        public IActionResult SignUp() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(string firstName, string lastName, string email, string password) {
            var user = await accountService.SignUpAsync(firstName, lastName, email, password);

            var claims = new List<Claim> {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new(ClaimTypes.Email, user.Email)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private IActionResult RedirectToLocal(string? returnUrl) {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

    }
}
