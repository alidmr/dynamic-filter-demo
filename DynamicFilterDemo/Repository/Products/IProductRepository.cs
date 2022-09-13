using DynamicFilterDemo.Entities;

namespace DynamicFilterDemo.Repository.Products
{
    public interface IProductRepository : IAsyncRepository<Product>, IRepository<Product>
    {
    }
}
