using System.Globalization;
using DynamicFilterDemo.Dynamics;
using DynamicFilterDemo.Filters;
using DynamicFilterDemo.Models.Users;
using DynamicFilterDemo.Services.Users;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DynamicFilterDemo.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetItems(UserSearchViewModel model, string parameters)
        {
            try
            {
                DynamicFilter dynamicFilter = null;

                if (!string.IsNullOrEmpty(parameters))
                {
                    var filterItems = JsonConvert.DeserializeObject<List<Filter>>(parameters);
                    var filter = new Filter("FirstName", "contains", "Kerem", "and", null);

                    var sortFilter = new List<SortFilter>()
                    {
                        new SortFilter(model.SortColumn,model.SortBy)
                    };
                    dynamicFilter = new DynamicFilter(sortFilter, filter);
                }

                var result = await _userService.GetUsers(dynamicFilter, model.Start, model.Length, model.Draw);
                return Json(result);
            }
            catch (Exception exception)
            {
                return Json(1);
            }
        }

        [HttpPost]
        public IActionResult GetUserList(UserSearchViewModel model, string parameters)
        {
            try
            {
                if (!string.IsNullOrEmpty(parameters))
                {
                    var filterItems = JsonConvert.DeserializeObject<List<AppFilterItem>>(parameters);

                    //2021-10-15 13:33:59.150
                    //var now = DateTime.Now;
                    //var todayStart = new DateTime(2021, 10, 15, 0, 0, 0);
                    //var todayEnd = new DateTime(2021, 10, 15, 23, 59, 59, 990);

                    //var filters = new List<AppFilterItem2>()
                    //{
                    //    new AppFilterItem2()
                    //    {
                    //        PropertyName = "CreatedDate",
                    //        OperatorType = (int)Operator.GreaterThanOrEqual,
                    //        Operator = Operator.GreaterThanOrEqual,
                    //        Value =todayStart.ToString()
                    //    },
                    //    new AppFilterItem2()
                    //    {
                    //        PropertyName = "CreatedDate",
                    //        OperatorType = (int)Operator.LessThanOrEqualTo,
                    //        Operator = Operator.LessThanOrEqualTo,
                    //        Value =todayEnd.ToString()
                    //    }
                    //};

                    model.Parameters = filterItems;
                }

                var result = _userService.GetUserList(model);

                return Json(result);
            }
            catch (Exception exception)
            {

                throw;
            }
        }


    }
}
