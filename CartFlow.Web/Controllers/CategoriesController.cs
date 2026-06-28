using CartFlow.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CartFlow.Web.Controllers;

public class CategoriesController(ICategoryService categoryService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var categories = await categoryService.GetAllWithHierarchyAsync();
        return View(categories);
    }
}
