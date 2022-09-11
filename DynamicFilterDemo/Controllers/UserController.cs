using DynamicFilterDemo.Dynamics;
using DynamicFilterDemo.Filters;
using DynamicFilterDemo.Models.Users;
using DynamicFilterDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
        public async Task<IActionResult> GetUsers(UserSearchViewModel model, string parameters)
        {
            try
            {
                if (!string.IsNullOrEmpty(parameters))
                {
                    var filterItems = JsonConvert.DeserializeObject<List<AppFilterItem>>(parameters);
                    model.Filters = filterItems;
                }

                var result = await _userService.GetUsers(model);

                return Json(result);
            }
            catch (Exception exception)
            {

                throw;
            }
        }

        [HttpPost]
        public IActionResult GetUserList(UserSearchViewModel model, string parameters)
        {
            try
            {
                if (!string.IsNullOrEmpty(parameters))
                {
                    var filterItems = JsonConvert.DeserializeObject<List<AppFilterItem2>>(parameters);
                    model.Parameters = filterItems;
                }

                var result =  _userService.GetUserList(model);

                return Json(result);
            }
            catch (Exception exception)
            {

                throw;
            }
        }


    }
}
