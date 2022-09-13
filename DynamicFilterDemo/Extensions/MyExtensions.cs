using DynamicFilterDemo.Filters;
using DynamicFilterDemo.Response;
using System.Linq;
using System.Linq.Expressions;

namespace DynamicFilterDemo.Extensions
{
    public static class MyExtensions
    {
        public static MyDataResponse<T> ToDataSource<T>(this IQueryable<T> query, List<AppFilterItem> filters, int page, int pageSize, string sortColumn = null, string sortBy = null)
        {
            MyDataResponse<T> source = new MyDataResponse<T>();

            if (filters != null && filters.Any())
            {
                Expression<Func<T, bool>> expression = MyExpressionBuilder.GetExpression<T>(filters);
                if (expression != null)
                {
                    query = query.Where(expression);
                }
            }

            query = query.GetSortedData(sortColumn, sortBy);

            source.Items = query.Skip(page).Take(pageSize).ToList();
            source.Count = query.Count();

            return source;
        }

        public static IQueryable<T> GetSortedData<T>(this IQueryable<T> data, string sortColumn, string sortBy)
        {
            if (string.IsNullOrEmpty(sortColumn) || string.IsNullOrEmpty(sortBy))
                return data;

            var param = Expression.Parameter(typeof(T), "item");

            var sortExpression = Expression.Lambda<Func<T, object>>
                (Expression.Convert(Expression.Property(param, sortColumn), typeof(object)), param);

            switch (sortBy.ToLower())
            {
                case "asc":
                    data = data.AsQueryable<T>().OrderBy<T, object>(sortExpression);
                    break;
                default:
                    data = data.AsQueryable<T>().OrderByDescending<T, object>(sortExpression);
                    break;
            }
            return data;
        }
    }
}
