using DynamicFilterDemo.Contexts;
using DynamicFilterDemo.Entities;

namespace DynamicFilterDemo.Repository.Categories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(FilterDemoContext context) : base(context)
        {
        }
    }
}
