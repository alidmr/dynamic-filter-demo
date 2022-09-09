using DynamicFilterDemo.Entities;

namespace DynamicFilterDemo.Repository.Users
{
    public interface IUserRepository : IAsyncRepository<User>, IRepository<User>
    {
    }
}
