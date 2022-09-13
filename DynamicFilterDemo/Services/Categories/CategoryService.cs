using Bogus;
using DynamicFilterDemo.Entities;
using DynamicFilterDemo.Extensions;
using DynamicFilterDemo.Models.Categories;
using DynamicFilterDemo.Models.Response;
using DynamicFilterDemo.Repository.Categories;

namespace DynamicFilterDemo.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public void SeedData()
        {
            var categories = new Faker<Category>("tr")
                .RuleFor(x => x.Name, x => x.Lorem.Word())
                .RuleFor(x => x.IsActive, x => x.Random.Bool())
                .RuleFor(x => x.IsDeleted, x => x.Random.Bool())
                .RuleFor(x => x.CreatedDate, x => x.Date.Between(DateTime.Now.AddDays(-20), DateTime.Now.AddDays(-5)))
                .Generate(10);

            _categoryRepository.AddRange(categories);
        }

        public BaseDataTableResponseModel<CategoryViewModel> GetCategories(CategorySearchViewModel model)
        {
            var response = new BaseDataTableResponseModel<CategoryViewModel>();

            var categoryQuery = _categoryRepository.Query();

            var query = (from c in categoryQuery
                         select new CategoryViewModel()
                         {
                             Id = c.Id,
                             Name = c.Name,
                             UpdatedDate = c.UpdatedDate,
                             CreatedDate = c.CreatedDate,
                             IsActive = c.IsActive,
                             IsDeleted = c.IsDeleted
                         });

            var result = query.ToDataSource(model.Parameters, model.Start, model.Length, model.SortColumn, model.SortBy);

            response.Draw = model.Draw;
            response.RecordsFiltered = result.Count;
            response.RecordsTotal = result.Count;
            response.Data = result.Items;

            return response;
        }
    }
}
