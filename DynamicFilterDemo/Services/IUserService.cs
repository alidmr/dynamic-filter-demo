using DynamicFilterDemo.Models.Response;
using DynamicFilterDemo.Models.Users;

namespace DynamicFilterDemo.Services
{
    public interface IUserService
    {
        Task SeedData();

        Task<BaseDataTableResponseModel<UserViewModel>> GetUsers(UserSearchViewModel model);
    }
}
