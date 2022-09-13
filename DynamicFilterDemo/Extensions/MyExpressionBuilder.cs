using System.Diagnostics.Metrics;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using DynamicFilterDemo.Filters;
using DynamicFilterDemo.Helpers;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace DynamicFilterDemo.Extensions
{
    public class MyExpressionBuilder
    {
        private static MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        private static MethodInfo startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
        private static MethodInfo endsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
        private static MethodInfo intListContainsMethod = typeof(List<int>).GetMethod("Contains", new[] { typeof(int) });
        private static MethodInfo intListNullableContainsMethod = typeof(List<int?>).GetMethod("Contains", new[] { typeof(int?) });
        private static MethodInfo listAnyMethod = typeof(Enumerable).GetMethods().First(t => t.Name == "Any" && t.GetParameters().Length == 2).MakeGenericMethod(typeof(int));


        public static Expression<Func<T, bool>> GetExpression<T>(List<AppFilterItem> filters)
        {
            // No filters passed in #KickIT
            if (filters.Count == 0)
                return null;

            // Create the parameter for the ObjectType (typically the 'x' in your expression (x => 'x')
            // The "parm" string is used strictly for debugging purposes
            ParameterExpression param = Expression.Parameter(typeof(T), "parm");

            // Store the result of a calculated Expression
            Expression exp = null;

            if (filters.Count == 1)
                exp = GetExpression<T>(param, filters[0]); // Create expression from a single instance
            else if (filters.Count == 2)
                exp = GetExpression<T>(param, filters[0], filters[1]); // Create expression that utilizes AndAlso mentality
            else
            {
                // Loop through filters until we have created an expression for each
                while (filters.Count > 0)
                {
                    // Grab initial filters remaining in our List
                    var f1 = filters[0];
                    var f2 = filters[1];

                    // Check if we have already set our Expression
                    if (exp == null)
                        exp = GetExpression<T>(param, filters[0], filters[1]); // First iteration through our filters
                    else
                        exp = Expression.AndAlso(exp, GetExpression<T>(param, filters[0], filters[1])); // Add to our existing expression

                    filters.Remove(f1);
                    filters.Remove(f2);

                    // Odd number, handle this seperately
                    if (filters.Count == 1)
                    {
                        // Pass in our existing expression and our newly created expression from our last remaining filter
                        exp = Expression.AndAlso(exp, GetExpression<T>(param, filters[0]));

                        // Remove filter to break out of while loop
                        filters.RemoveAt(0);
                    }
                }
            }

            return Expression.Lambda<Func<T, bool>>(exp, param);
        }


        private static Expression GetExpression<T>(ParameterExpression param, AppFilterItem filter)
        {
            // The member you want to evaluate (x => x.FirstName)
            // where yapılacak property alınıyor
            MemberExpression member = Expression.Property(param, filter.PropertyName);

            Type type = typeof(T).GetProperty($"{filter.PropertyName}")?.PropertyType!;

            // The value you want to evaluate
            // where şartında ele alınacak değer (search key)
            //ConstantExpression constant = Expression.Constant(filter.Value);
            ConstantExpression constant = null;

            //object filterValue = filter.Value;

            object filterValue = QueryValueConverter.GetValue(filter.Value);

            object newValue = null;

            if (type == typeof(DateTime) || type == typeof(DateTime?) && filterValue != null)
            {
                if (DateTime.TryParse(filterValue.ToString(), out DateTime date))
                {
                    newValue = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
                }
            }
            else if ((type != typeof(int) && type != typeof(List<int>) && type != typeof(int?)) || filter.Operator != Operator.Equals)
            {
                newValue = AppHelper.ConvertValue(filterValue, type);
            }

            // Determine how we want to apply the expression
            switch (filter.Operator)
            {
                case Operator.Equals:
                    if (type == typeof(int) && filterValue != null && filterValue.ToString().Contains(","))
                    {
                        var stringValues = filterValue.ToString().Split(",", StringSplitOptions.RemoveEmptyEntries);

                        List<int> values = new List<int>();

                        for (int i = 0; i < stringValues.Length; i++)
                        {
                            values.Add(Convert.ToInt32(stringValues[i]));
                        }

                        constant = Expression.Constant(values);
                        var item = Expression.Call(constant, intListContainsMethod, member);
                        return item;
                    }
                    else if (type == typeof(int?) && filterValue != null && filterValue.ToString().Contains(","))
                    {
                        var stringValues = filterValue.ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                        List<int?> values = new List<int?>();

                        for (int i = 0; i < stringValues.Length; i++)
                        {
                            values.Add(Convert.ToInt32(stringValues[i]));
                        }

                        constant = Expression.Constant(values);

                        var item = Expression.Call(constant, intListNullableContainsMethod, member);
                        return item;
                    }
                    else if (typeof(IEnumerable<int>).IsAssignableFrom(type) && filterValue != null)
                    {
                        if (filterValue.ToString().Contains(","))
                        {
                            var stringValues = filterValue.ToString().Split(",", StringSplitOptions.RemoveEmptyEntries);

                            List<int> values = new List<int>();

                            List<int> values2 = new List<int>();
                            values.Any(p => values2.Contains(p));

                            for (int i = 0; i < stringValues.Length; i++)
                            {
                                values.Add(Convert.ToInt32(stringValues[i]));
                            }

                            Func<int, bool> predicate = p => values.Contains(p);

                            Expression<Func<int, bool>> exp = e => predicate(e);

                            var item = Expression.Call(null, listAnyMethod, member, exp);
                            return item;
                        }
                        else
                        {
                            List<int> values = new List<int>();

                            values.Add(Convert.ToInt32(filterValue));


                            Func<int, bool> predicate = p => values.Contains(p);

                            Expression<Func<int, bool>> exp = e => predicate(e);

                            var item = Expression.Call(null, listAnyMethod, member, exp);
                            return item;
                        }
                    }
                    else
                    {
                        if (newValue == null)
                        {
                            newValue = AppHelper.ConvertValue(filterValue, type);
                        }
                        constant = Expression.Constant(newValue);
                        var item = Expression.Equal(member, constant);
                        return item;
                    }
                case Operator.NotEqual:
                    constant = Expression.Constant(newValue);
                    return Expression.NotEqual(member, constant);

                case Operator.IsFalse:
                    return Expression.Equal(member, Expression.Constant(false));

                case Operator.IsTrue:
                    return Expression.Equal(member, Expression.Constant(true));

                case Operator.Contains:
                    constant = Expression.Constant(newValue);
                    return Expression.Call(member, containsMethod, constant);

                case Operator.GreaterThan:
                    constant = Expression.Constant(newValue);
                    return Expression.GreaterThan(member, constant);

                case Operator.GreaterThanOrEqual:
                    constant = Expression.Constant(newValue, type);
                    return Expression.GreaterThanOrEqual(member, constant);

                case Operator.LessThan:
                    constant = Expression.Constant(newValue);
                    return Expression.LessThan(member, constant);

                case Operator.LessThanOrEqualTo:
                    constant = Expression.Constant(newValue, type);
                    return Expression.LessThanOrEqual(member, constant);

                case Operator.StartsWith:
                    constant = Expression.Constant(newValue);
                    return Expression.Call(member, startsWithMethod, constant);

                case Operator.EndsWith:
                    constant = Expression.Constant(newValue);
                    return Expression.Call(member, endsWithMethod, constant);
            }
            return null;
        }

        private static BinaryExpression GetExpression<T>(ParameterExpression param, AppFilterItem filter1, AppFilterItem filter2)
        {
            Expression result1 = GetExpression<T>(param, filter1);
            Expression result2 = GetExpression<T>(param, filter2);
            return Expression.AndAlso(result1, result2);
        }
    }
}
