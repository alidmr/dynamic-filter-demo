using DynamicFilterDemo.Filters;
using DynamicFilterDemo.Models.Categories;
using DynamicFilterDemo.Services.Categories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DynamicFilterDemo.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetCategories(CategorySearchViewModel model, string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                model.Parameters = JsonConvert.DeserializeObject<List<AppFilterItem>>(parameters);
            }
            var result = _categoryService.GetCategories(model);
            return Json(result);
        }
    }
}
