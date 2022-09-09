using System.Linq.Expressions;
using DynamicFilterDemo.Paging;
using Microsoft.EntityFrameworkCore.Query;

namespace DynamicFilterDemo.Repository
{

    public interface IAsyncRepository<T> : IQuery<T>
    {
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate);

        Task<IPaginate<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            int index = 0, int size = 10, bool enableTracking = true,
            CancellationToken cancellationToken = default);

        Task<IPaginate<T>> GetListByDynamicAsync(Dynamics.DynamicFilter dynamic,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            int index = 0, int size = 10, bool enableTracking = true,
            CancellationToken cancellationToken = default);

        Task<T> AddAsync(T entity);

        Task AddRangeAsync(List<T> entities);

        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(T entity);
    }
}
