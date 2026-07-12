using CartFlow.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CartFlow.Web.ViewComponents
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly ICategoryService _categoryService;

        public CategoryMenuViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _categoryService.GetAllWithHierarchyAsync();
            return View(categories);
        }
    }
}
