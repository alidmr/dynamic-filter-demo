using Bogus;
using DynamicFilterDemo.Entities;
using DynamicFilterDemo.Extensions;
using DynamicFilterDemo.Models.Products;
using DynamicFilterDemo.Models.Response;
using DynamicFilterDemo.Repository.Categories;
using DynamicFilterDemo.Repository.Products;
using DynamicFilterDemo.Repository.Users;

namespace DynamicFilterDemo.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, IUserRepository userRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
        }

        public void SeedData()
        {
            var categoryIds = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var userIds = new int[] { 1, 2, 3, 4, 5 };

            var products = new Faker<Product>("tr")
                .RuleFor(x => x.Name, x => x.Commerce.ProductName())
                .RuleFor(x => x.Description, x => x.Commerce.ProductDescription())
                .RuleFor(x => x.Price, x => Convert.ToDecimal(x.Commerce.Price()))
                .RuleFor(x => x.IsActive, x => x.Random.Bool())
                .RuleFor(x => x.IsDeleted, x => x.Random.Bool())
                .RuleFor(x => x.CreatedDate, x => x.Date.Between(DateTime.Now.AddDays(-20), DateTime.Now.AddDays(-5)))
                .RuleFor(x => x.CategoryId, x => x.PickRandom(categoryIds))
                .RuleFor(x => x.UserId, x => x.PickRandom(userIds))

                .Generate(50);

            _productRepository.AddRange(products);
        }

        public BaseDataTableResponseModel<ProductViewModel> GetProducts(ProductSearchViewModel model)
        {
            var response = new BaseDataTableResponseModel<ProductViewModel>();

            var productQuery = _productRepository.Query();
            var categoryQuery = _categoryRepository.Query();
            var userQuery = _userRepository.Query();

            var query = (from p in productQuery
                         join c in categoryQuery on p.CategoryId equals c.Id
                         join u in userQuery on p.UserId equals u.Id
                         select new ProductViewModel()
                         {
                             Id = p.Id,
                             Price = p.Price,
                             IsActive = p.IsActive,
                             IsDeleted = p.IsDeleted,
                             UpdatedDate = p.UpdatedDate,
                             CategoryId = p.CategoryId,
                             CategoryName = c.Name,
                             CreatedDate = p.CreatedDate,
                             Description = p.Description,
                             Name = p.Name,
                             UserId = p.UserId,
                             UserName = u.FirstName
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
