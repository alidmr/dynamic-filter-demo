using DynamicFilterDemo.Contexts;
using DynamicFilterDemo.Entities;

namespace DynamicFilterDemo.Repository.Products
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(FilterDemoContext context) : base(context)
        {
        }
    }
}
