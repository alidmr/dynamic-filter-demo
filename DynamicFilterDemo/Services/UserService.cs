using Bogus;
using DynamicFilterDemo.Entities;
using DynamicFilterDemo.Models.Response;
using DynamicFilterDemo.Models.Users;
using DynamicFilterDemo.Repository.Users;

namespace DynamicFilterDemo.Services
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

        public async Task<BaseDataTableResponseModel<UserViewModel>> GetUsers(UserSearchViewModel model)
        {
            var response = new BaseDataTableResponseModel<UserViewModel>();

            var result = await _userRepository.GetListAsync(null, null, null, model.Start, model.Length);

            response.Draw = model.Draw;
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
    }
}
