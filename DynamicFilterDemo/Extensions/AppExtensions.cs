using DynamicFilterDemo.Dynamics;
using System.Linq.Dynamic.Core;
using System.Text;
using DynamicFilterDemo.Filters;
using static NuGet.Packaging.PackagingConstants;

namespace DynamicFilterDemo.Extensions
{
    public static class AppExtensions
    {
        private static readonly IDictionary<string, string> Operators = new Dictionary<string, string>
            {
                { "eq", "=" },
                { "neq", "!=" },
                { "lt", "<" },
                { "lte", "<=" },
                { "gt", ">" },
                { "gte", ">=" },
                { "isnull", "== null" },
                { "isnotnull", "!= null" },
                { "startswith", "StartsWith" },
                { "endswith", "EndsWith" },
                { "contains", "Contains" },
                { "doesnotcontain", "Contains" }
            };

        public static IQueryable<T> ToFilterQuery<T>(this IQueryable<T> query, List<AppFilterItem>? filterItems)
        {
            if (filterItems != null && filterItems.Any())
            {
                query = Filter(query, filterItems);
            }

            //    if (dynamic.Sort is not null && dynamic.Sort.Any())
            //    {
            //        query = Sort(query, dynamic.Sort);
            //    }

            return query;
        }

        private static IQueryable<T> Filter<T>(IQueryable<T> queryable, List<AppFilterItem> filterItems)
        {
            //IList<AppFilterItem> filters = GetAllFilters(filter);

            string?[] values = filterItems.Select(f => f.Value).ToArray();

            string where = Ali(filterItems);

            queryable = queryable.Where(where, values);

            return queryable;
        }


        private static string Ali(IList<AppFilterItem> filterItems)
        {
            //StringBuilder where = new();
            int index = 0;
            var query = string.Empty;

            foreach (var filter in filterItems)
            {
                StringBuilder sb = new();
                string logic = filter.Logic;
                string comparison = Operators[filter.Operator];

                if (!string.IsNullOrEmpty(filter.Value))
                {
                    if (filter.Operator == "doesnotcontain")
                    {
                        sb.Append($"(!np({filter.Field}).{comparison}(@{index}))");
                    }
                    else if (comparison == "StartsWith" || comparison == "EndsWith" || comparison == "Contains")
                    {
                        sb.Append($"(np({filter.Field}).{comparison}(@{index}))");
                    }
                    else
                    {
                        sb.Append($"np({filter.Field}) {comparison} @{index}");
                    }
                }
                else if (filter.Operator == "isnull" || filter.Operator == "isnotnull")
                {
                    sb.Append($"np({filter.Field}) {comparison}");
                }

                if (index > 0)
                {
                    var item = $"{query} {logic} ({sb})";
                    query = item;
                }
                else
                {
                    query = sb.ToString();
                }
                index++;
            }

            return query;
        }


        private static string Transform(AppFilterItem? filter, IList<AppFilterItem> filters, bool isFlag = true)
        {
            StringBuilder where = new();

            if (filter is not null)
            {
                int index = filters.IndexOf(filter);
                string comparison = Operators[filter.Operator];

                if (!string.IsNullOrEmpty(filter.Value))
                {
                    if (filter.Operator == "doesnotcontain")
                    {
                        where.Append($"(!np({filter.Field}).{comparison}(@{index}))");
                    }
                    else if (comparison == "StartsWith" ||
                             comparison == "EndsWith" ||
                             comparison == "Contains")
                    {
                        where.Append($"(np({filter.Field}).{comparison}(@{index}))");
                    }
                    else
                    {
                        where.Append($"np({filter.Field}) {comparison} @{index}");
                    }
                }
                else if (filter.Operator == "isnull" || filter.Operator == "isnotnull")
                {
                    where.Append($"np({filter.Field}) {comparison}");
                }
            }

            if (isFlag && filters.Any())
            {
                var array = filters.Select(f => Transform(f, filters, false)).ToArray();
                var item = $"{where} {filter.Logic} ({string.Join($" {filter.Logic} ", array)})";
                return item;
            }

            return where.ToString();
        }


        public static IList<Filter> GetAllFilters(Filter filter)
        {
            List<Filter> filters = new();
            GetFilters(filter, filters);
            return filters;
        }

        private static void GetFilters(Filter filter, IList<Filter> filters)
        {
            filters.Add(filter);
            if (filter.Filters is not null && filter.Filters.Any())
            {
                foreach (Filter item in filter.Filters)
                {
                    GetFilters(item, filters);
                }
            }
        }


        //private static IQueryable<T> Sort<T>(IQueryable<T> queryable, IEnumerable<SortFilter> sort)
        //{
        //    if (sort.Any())
        //    {
        //        string ordering = string.Join(",", sort.Select(s => $"{s.Field} {s.Dir}"));
        //        return queryable.OrderBy(ordering);
        //    }

        //    return queryable;
        //}

        public static IQueryable<T> Sort<T>(this IQueryable<T> queryable, string? sortColumn, string? sortBy)
        {
            if (string.IsNullOrEmpty(sortColumn) || string.IsNullOrEmpty(sortBy))
                return queryable;

            string ordering = $"{sortColumn} {sortBy}";
            return queryable.OrderBy(ordering);

        }
    }
}
