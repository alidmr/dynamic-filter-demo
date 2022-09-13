using Bogus;
using Bogus.DataSets;
using DynamicFilterDemo.Dynamics;
using DynamicFilterDemo.Entities;
using DynamicFilterDemo.Extensions;
using DynamicFilterDemo.Models.Response;
using DynamicFilterDemo.Models.Users;
using DynamicFilterDemo.Repository.Users;

namespace DynamicFilterDemo.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task SeedData()
        {
            var users = TestSeedData();

            await _userRepository.AddRangeAsync(users);
        }

        private List<User> TestSeedData()
        {
            var testData = new Faker<User>("tr")
                    .RuleFor(x => x.FirstName, x => x.Person.FirstName)
                    .RuleFor(x => x.LastName, x => x.Person.LastName)
                    .RuleFor(x => x.Email, x => x.Person.Email)
                    .RuleFor(x => x.Age, x => x.Random.Int(15, 90))
                    .RuleFor(x => x.IsActive, x => x.Random.Bool())
                    .RuleFor(x => x.IsDeleted, x => x.Random.Bool())
                    .RuleFor(x => x.CreatedDate, x => x.Date.Between(DateTime.Now.AddDays(-20), DateTime.Now.AddDays(-100)))
                    .RuleFor(x => x.UpdatedDate, x => x.Date.Between(DateTime.Now, DateTime.Now.AddDays(-20)))

                    .Generate(3000);

            return testData;
        }

        public async Task<BaseDataTableResponseModel<UserViewModel>> GetUsers(DynamicFilter filter, int page, int pageSize, int draw)
        {
            var response = new BaseDataTableResponseModel<UserViewModel>();

            var userQuery = _userRepository.Query();

            var query = (from u in userQuery
                         select new UserViewModel()
                         {
                             Id = u.Id,
                             FirstName = u.FirstName,
                             LastName = u.LastName,
                             Email = u.Email,
                             Age = u.Age,
                             CreatedDate = u.CreatedDate,
                             IsActive = u.IsActive,
                             IsDeleted = u.IsDeleted,
                             UpdatedDate = u.UpdatedDate
                         }).AsQueryable();

            var items = query.ToDynamic(filter);

            var result = await items.ToPaginateAsync(page, pageSize);

            //var result = await _userRepository.GetListByDynamicAsync(filter, null, page, pageSize);

            response.Draw = draw;
            response.RecordsFiltered = result.Count;
            response.RecordsTotal = result.Count;
            response.Data = result.Items.Select(x => new UserViewModel()
            {
                Id = x.Id,
                UpdatedDate = x.UpdatedDate,
                Age = x.Age,
                CreatedDate = x.CreatedDate,
                Email = x.Email,
                FirstName = x.FirstName,
                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted,
                LastName = x.LastName
            }).ToList();

            return response;
        }


        public BaseDataTableResponseModel<UserViewModel> GetUserList(UserSearchViewModel model)
        {
            var response = new BaseDataTableResponseModel<UserViewModel>();

            var userQuery = _userRepository.Query();

            var query = (from u in userQuery
                         select new UserViewModel()
                         {
                             Id = u.Id,
                             FirstName = u.FirstName,
                             LastName = u.LastName,
                             Email = u.Email,
                             Age = u.Age,
                             CreatedDate = u.CreatedDate,
                             IsActive = u.IsActive,
                             IsDeleted = u.IsDeleted,
                             UpdatedDate = u.UpdatedDate
                         }).AsQueryable();

            var result = query.ToDataSource(model.Parameters, model.Start, model.Length);

            response.Draw = model.Draw;
            response.RecordsFiltered = result.Count;
            response.RecordsTotal = result.Count;
            response.Data = result.Items;

            return response;
        }
    }
}
