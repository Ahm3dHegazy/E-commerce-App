using CartFlow.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CartFlow.Web.Controllers {
    public class CategoriesController : Controller {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService) {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index() {
            var categories = await _categoryService.GetAllWithHierarchyAsync();
            return View(categories);
        }
    }
}
