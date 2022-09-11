using DynamicFilterDemo.Filters;
using DynamicFilterDemo.Response;
using System.Linq.Expressions;

namespace DynamicFilterDemo.Extensions
{
    public static class MyExtensions
    {
        public static MyDataResponse<T> ToDataSource<T>(this IQueryable<T> query, List<AppFilterItem2> filters, int page, int pageSize)
        {
            MyDataResponse<T> source = new MyDataResponse<T>();

            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    if (filter.Operator == Operator.Contains || filter.Operator == Operator.StartsWith ||
                        filter.Operator == Operator.EndsWith)
                    {
                        filter.Value = filter.Value.ToUpper();
                    }
                }

                Expression<Func<T, bool>> expression = MyExpressionBuilder.GetExpression<T>(filters);
                if (expression != null)
                {
                    query = query.Where(expression);
                }
            }

            source.Items = query.Skip(page).Take(pageSize).ToList();
            source.Count = query.Count();

            return source;
        }
    }
}
