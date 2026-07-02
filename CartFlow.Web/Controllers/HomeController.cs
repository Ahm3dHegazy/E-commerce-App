using CartFlow.Services.Interfaces;
using CartFlow.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CartFlow.Web.Controllers {
    public class HomeController : Controller {
        private readonly IProductService _productService;

        public HomeController(IProductService productService) {
            _productService = productService;
        }

        public async Task<IActionResult> Index() {
            ViewBag.FeaturedProducts = await _productService.GetFeaturedAsync(8);
            return View();
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}
