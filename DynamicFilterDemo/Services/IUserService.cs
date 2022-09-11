﻿using DynamicFilterDemo.Dynamics;
using DynamicFilterDemo.Models.Response;
using DynamicFilterDemo.Models.Users;

namespace DynamicFilterDemo.Services
{
    public interface IUserService
    {
        Task SeedData();

        Task<BaseDataTableResponseModel<UserViewModel>> GetUsers(UserSearchViewModel model);

        Task<BaseDataTableResponseModel<UserViewModel>> GetUsers(DynamicFilter filter, int page, int pageSize, int draw);

        BaseDataTableResponseModel<UserViewModel> GetUserList(UserSearchViewModel model);
    }
}
