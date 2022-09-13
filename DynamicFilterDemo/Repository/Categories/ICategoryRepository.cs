using DynamicFilterDemo.Entities;

namespace DynamicFilterDemo.Repository.Categories
{
    public interface ICategoryRepository : IAsyncRepository<Category>, IRepository<Category>
    {

    }
}
