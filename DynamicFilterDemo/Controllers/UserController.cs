using DynamicFilterDemo.Models.Users;
using DynamicFilterDemo.Services;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetItems(UserSearchViewModel model)
        {
            var result = await _userService.GetUsers(model);
            return Json(result);
        }


        private async Task SeedData()
        {
            await _userService.SeedData();
        }

    }
}
