using DynamicFilterDemo.Models.Categories;
using DynamicFilterDemo.Models.Response;

namespace DynamicFilterDemo.Services.Categories
{
    public interface ICategoryService
    {
        void SeedData();

        BaseDataTableResponseModel<CategoryViewModel> GetCategories(CategorySearchViewModel model);
    }
}
