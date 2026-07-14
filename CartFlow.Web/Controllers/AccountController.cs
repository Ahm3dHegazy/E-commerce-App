using CartFlow.Web.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace CartFlow.Web.Controllers
{
    using CartFlow.Services.Interfaces;
    using CartFlow.Web.Extensions;
    using Microsoft.AspNetCore.Authentication;

    public class AccountController(IAccountService accountService, ICartService cartService, IEmailService emailService) : Controller
    {
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                var token = await accountService.GeneratePasswordResetTokenAsync(email);
                if (token is not null)
                {
                    var resetLink = Url.Action(nameof(ResetPassword), "Account",
                        new { email, token }, protocol: Request.Scheme);

                    await emailService.SendPasswordResetEmailAsync(email, resetLink!);
                }
            }

            // Always show the same message, whether or not the email exists,
            // so we don't reveal which addresses are registered.
            ViewData["Submitted"] = true;
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction(nameof(ForgotPassword));
            }

            ViewData["email"] = email;
            ViewData["token"] = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string email, string token, string newPassword, string confirmPassword)
        {
            ViewData["email"] = email;
            ViewData["token"] = token;

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                ModelState.AddModelError("", "Password must be at least 6 characters.");
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View();
            }

            var success = await accountService.ResetPasswordAsync(email, token, newPassword);
            if (!success)
            {
                ModelState.AddModelError("", "This reset link is invalid or has expired. Please request a new one.");
                return View();
            }

            TempData["Success"] = "Your password has been reset. Please sign in.";
            return RedirectToAction(nameof(SignIn));
        }

        [HttpGet]
        public IActionResult GoogleLogin(string? returnUrl)
        {
            var redirectUrl = Url.Action(nameof(GoogleResponse), "Account", new { returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, Microsoft.AspNetCore.Authentication.Google.GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse(string? returnUrl)
        {
            var result = await HttpContext.AuthenticateAsync("External");
            if (!result.Succeeded || result.Principal is null)
            {
                TempData["Error"] = "Google sign-in failed. Please try again.";
                return RedirectToAction(nameof(SignIn));
            }

            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Google didn't share an email address, so we couldn't sign you in.";
                return RedirectToAction(nameof(SignIn));
            }

            var firstName = result.Principal.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;
            var lastName = result.Principal.FindFirstValue(ClaimTypes.Surname) ?? string.Empty;

            var user = await accountService.FindOrCreateExternalUserAsync(email, firstName, lastName);

            var claims = new List<Claim> {
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
        new(ClaimTypes.Email, user.Email)
    };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Merge guest session cart (if any) into user's DB cart, same as regular sign-in
            try
            {
                var guestVm = HttpContext.Session.GetObject<List<CartFlow.Web.Models.CartItemViewModel>>("Cart");
                if (guestVm != null && guestVm.Any())
                {
                    var dto = guestVm.Select(g => new GuestCartItemDto { ProductId = g.ProductId, Quantity = g.Quantity }).ToList();
                    await cartService.MergeGuestCartAsync(user.Id, dto);
                    HttpContext.Session.Remove("Cart");
                }
            }
            catch
            {
                // Non-fatal: merging failed; proceed with sign-in so user isn't blocked
            }

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            await HttpContext.SignOutAsync("External");

            return RedirectToLocal(returnUrl);
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(string email, string password, string? returnUrl)
        {
            var user = await accountService.SignInAsync(email, password);
            if (user is null)
            {
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

            // Merge guest session cart (if any) into user's DB cart before finalizing sign-in
            try
            {
                var guestVm = HttpContext.Session.GetObject<List<CartFlow.Web.Models.CartItemViewModel>>("Cart");
                if (guestVm != null && guestVm.Any())
                {
                    var dto = guestVm.Select(g => new GuestCartItemDto { ProductId = g.ProductId, Quantity = g.Quantity }).ToList();
                    await cartService.MergeGuestCartAsync(user.Id, dto);
                    HttpContext.Session.Remove("Cart");
                }
            }
            catch
            {
                // Non-fatal: merging failed; proceed with sign-in so user isn't blocked
            }

            await HttpContext.SignInAsync(principal);

            return RedirectToLocal(returnUrl);
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(string firstName, string lastName, string email, string password)
        {
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

                // Merge guest cart into newly created user cart
                try
                {
                    var guestVm = HttpContext.Session.GetObject<List<CartFlow.Web.Models.CartItemViewModel>>("Cart");
                    if (guestVm != null && guestVm.Any())
                    {
                        var dto = guestVm.Select(g => new GuestCartItemDto { ProductId = g.ProductId, Quantity = g.Quantity }).ToList();
                        await cartService.MergeGuestCartAsync(user.Id, dto);
                        HttpContext.Session.Remove("Cart");
                    }
                }
                catch
                {
                    // proceed even if merge fails
                }

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

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

    }
}
