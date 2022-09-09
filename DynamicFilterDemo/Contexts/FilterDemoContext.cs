using DynamicFilterDemo.Entities;
using Microsoft.EntityFrameworkCore;

namespace DynamicFilterDemo.Contexts
{
    public class FilterDemoContext : DbContext
    {
        public FilterDemoContext(DbContextOptions<FilterDemoContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }

    }
}
