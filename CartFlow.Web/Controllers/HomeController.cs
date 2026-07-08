using CartFlow.Services.Interfaces;
using CartFlow.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CartFlow.Web.Controllers {
    public class HomeController : Controller {
        private readonly IProductService _productService;
        private readonly IReviewService _reviewService;

        public HomeController(IProductService productService, IReviewService reviewService) {
            _productService = productService;
            _reviewService = reviewService;
        }

        public async Task<IActionResult> Index() {
            var products = await _productService.GetFeaturedAsync(6);

            var viewModels = products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                UnitPrice = p.UnitPrice,
                StockQuantity = p.StockQuantity,
                CategoryName = p.Category?.Name,
                CategoryId = p.CategoryId,
                ImageUrl = p.ProductImages?.FirstOrDefault(pi => pi.IsPrimary)?.Image
                    ?? p.ProductImages?.FirstOrDefault()?.Image,
                ImageUrls = p.ProductImages?.Select(pi => pi.Image).ToList() ?? new(),
                Initial = string.IsNullOrEmpty(p.Name) ? string.Empty : p.Name[0].ToString().ToUpper()
            }).ToList();

            foreach (var vm in viewModels)
            {
                vm.Reviews = (await _reviewService.GetReviewsForProductAsync(vm.Id)).ToList();
            }

            ViewBag.FeaturedProducts = viewModels;
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
