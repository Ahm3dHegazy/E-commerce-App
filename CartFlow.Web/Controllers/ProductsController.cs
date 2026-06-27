using CartFlow.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CartFlow.Web.Controllers {
    public class ProductsController : Controller {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService) {
            _productService = productService;
        }

        public async Task<IActionResult> Index() {
            var products = await _productService.GetAllAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(int id) {
            var product = await _productService.GetByIdAsync(id);
            if (product is null) return NotFound();
            return View(product);
        }
    }
}
