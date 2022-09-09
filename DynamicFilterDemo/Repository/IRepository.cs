using System.Linq.Expressions;
using DynamicFilterDemo.Paging;
using Microsoft.EntityFrameworkCore.Query;

namespace DynamicFilterDemo.Repository
{
    public interface IRepository<T> : IQuery<T>
    {
        T Get(Expression<Func<T, bool>> predicate);

        IPaginate<T> GetList(Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            int index = 0, int size = 10,
            bool enableTracking = true);

        IPaginate<T> GetListByDynamic(Dynamics.DynamicFilter dynamic,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            int index = 0, int size = 10, bool enableTracking = true);

        T Add(T entity);
        void AddRange(List<T> entities);

        T Update(T entity);
        T Delete(T entity);
    }
}
