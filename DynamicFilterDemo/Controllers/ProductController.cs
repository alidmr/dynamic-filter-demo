using DynamicFilterDemo.Filters;
using DynamicFilterDemo.Models.Products;
using DynamicFilterDemo.Services.Products;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DynamicFilterDemo.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetProducts(ProductSearchViewModel model, string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                // 2021-11-23 15:36:59.020
                var startDate = new DateTime(2021, 11, 23, 00, 00, 00);
                var endDate = new DateTime(2021, 11, 23, 23, 59, 59);
                var filters = new List<AppFilterItem>
                {
                    new AppFilterItem()
                    {
                        Operator = Operator.GreaterThanOrEqual,
                        OperatorType = 3,
                        PropertyName = "CreatedDate",
                        Value = startDate.ToString()
                    },
                    new AppFilterItem()
                    {
                        Operator = Operator.LessThanOrEqualTo,
                        OperatorType = 5,
                        PropertyName = "CreatedDate",
                        Value = endDate.ToString()
                    }
                };

                //model.Parameters = JsonConvert.DeserializeObject<List<AppFilterItem2>>(parameters);
                model.Parameters = filters;
            }

            var result = _productService.GetProducts(model);
            return Json(result);
        }

    }
}
