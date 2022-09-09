using DynamicFilterDemo.Contexts;
using DynamicFilterDemo.Entities;

namespace DynamicFilterDemo.Repository.Users
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(FilterDemoContext context) : base(context)
        {
        }
    }
}
