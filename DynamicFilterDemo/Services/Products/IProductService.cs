using DynamicFilterDemo.Models.Products;
using DynamicFilterDemo.Models.Response;

namespace DynamicFilterDemo.Services.Products
{
    public interface IProductService
    {
        void SeedData();

        BaseDataTableResponseModel<ProductViewModel> GetProducts(ProductSearchViewModel model);
    }
}
