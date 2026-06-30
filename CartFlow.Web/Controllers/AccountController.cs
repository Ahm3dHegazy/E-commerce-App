using CartFlow.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CartFlow.Web.Controllers {
    public class AccountController : Controller {
        public AccountController(IAccountService accountService) {
        }

        public IActionResult SignIn() {
            return View();
        }

        public IActionResult SignUp() {
            return View();
        }
    }
}
