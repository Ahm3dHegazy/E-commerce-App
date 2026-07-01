using CartFlow.Services.Interfaces;
using CartFlow.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;


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
            try
            {
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
            catch (DbUpdateException dbEx)
            {
                // Preserve entered values so the form is not cleared
                ViewData["firstName"] = firstName;
                ViewData["lastName"] = lastName;
                ViewData["email"] = email;

                // Detect unique index violation for Email (SQL Server index name used: IX_Users_Email)
                var inner = dbEx.InnerException?.Message ?? string.Empty;
                if (inner.Contains("IX_Users_Email") || inner.Contains("duplicate key") || inner.Contains("Unique index"))
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                    return View();
                }

                ModelState.AddModelError(string.Empty, "An error occurred while creating your account.");
                return View();
            }
            catch (Exception)
            {
                ViewData["firstName"] = firstName;
                ViewData["lastName"] = lastName;
                ViewData["email"] = email;

                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("SignIn");

            var user = await accountService.GetByIdAsync(int.Parse(userIdString));
            if (user is null) return RedirectToAction("SignIn");

            return View(new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone
            });
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("SignIn");

            var user = await accountService.UpdateProfileAsync(int.Parse(userIdString), model.FirstName, model.LastName, model.Email, model.Phone);
            if (user is null) return NotFound();

            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction("Profile");
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
