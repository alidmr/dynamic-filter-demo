using DynamicFilterDemo.Contexts;
using DynamicFilterDemo.Paging;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using DynamicFilterDemo.Extensions;

namespace DynamicFilterDemo.Repository
{
    public class Repository<TEntity> : IAsyncRepository<TEntity>, IRepository<TEntity> where TEntity : class
    {
        protected FilterDemoContext Context;

        public Repository(FilterDemoContext context)
        {
            Context = context;
        }


        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }

        public async Task<IPaginate<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null,
                                                           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy =
                                                               null,
                                                           Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>?
                                                               include = null,
                                                           int index = 0, int size = 10, bool enableTracking = true,
                                                           CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> queryable = Query();
            if (!enableTracking) queryable = queryable.AsNoTracking();
            if (include != null) queryable = include(queryable);
            if (predicate != null) queryable = queryable.Where(predicate);
            if (orderBy != null)
                return await orderBy(queryable).ToPaginateAsync(index, size, 0, cancellationToken);
            return await queryable.ToPaginateAsync(index, size, 0, cancellationToken);
        }

        public async Task<IPaginate<TEntity>> GetListByDynamicAsync(Dynamics.DynamicFilter dynamic,
                                                                    Func<IQueryable<TEntity>,
                                                                            IIncludableQueryable<TEntity, object>>?
                                                                        include = null,
                                                                    int index = 0, int size = 10,
                                                                    bool enableTracking = true,
                                                                    CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> queryable = Query().AsQueryable().ToDynamic(dynamic);
            if (!enableTracking) queryable = queryable.AsNoTracking();
            if (include != null) queryable = include(queryable);
            return await queryable.ToPaginateAsync(index, size, 0, cancellationToken);
        }

        public IQueryable<TEntity> Query()
        {
            return Context.Set<TEntity>();
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Added;
            await Context.SaveChangesAsync();
            return entity;
        }

        public async Task AddRangeAsync(List<TEntity> entities)
        {
            await Context.Set<TEntity>().AddRangeAsync(entities);
            await Context.SaveChangesAsync();
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            await Context.SaveChangesAsync();
            return entity;
        }

        public async Task<TEntity> DeleteAsync(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Deleted;
            await Context.SaveChangesAsync();
            return entity;
        }

        public TEntity? Get(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().FirstOrDefault(predicate);
        }

        public IPaginate<TEntity> GetList(Expression<Func<TEntity, bool>>? predicate = null,
                                          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
                                          int index = 0, int size = 10,
                                          bool enableTracking = true)
        {
            IQueryable<TEntity> queryable = Query();
            if (!enableTracking) queryable = queryable.AsNoTracking();
            if (include != null) queryable = include(queryable);
            if (predicate != null) queryable = queryable.Where(predicate);
            if (orderBy != null)
                return orderBy(queryable).ToPaginate(index, size);
            return queryable.ToPaginate(index, size);
        }

        public IPaginate<TEntity> GetListByDynamic(Dynamics.DynamicFilter dynamic,
                                                   Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>?
                                                       include = null, int index = 0, int size = 10,
                                                   bool enableTracking = true)
        {
            IQueryable<TEntity> queryable = Query().AsQueryable().ToDynamic(dynamic);
            if (!enableTracking) queryable = queryable.AsNoTracking();
            if (include != null) queryable = include(queryable);
            return queryable.ToPaginate(index, size);
        }

        public TEntity Add(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Added;
            Context.SaveChanges();
            return entity;
        }

        public void AddRange(List<TEntity> entities)
        {
            Context.Set<TEntity>().AddRange(entities);
            Context.SaveChanges();
        }

        public TEntity Update(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            Context.SaveChanges();
            return entity;
        }

        public TEntity Delete(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Deleted;
            Context.SaveChanges();
            return entity;
        }
    }
}
