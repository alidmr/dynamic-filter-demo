using System.Linq.Expressions;
using System.Reflection;
using DynamicFilterDemo.Filters;
using Microsoft.AspNetCore.Http.Extensions;

namespace DynamicFilterDemo.Extensions
{
    public class MyExpressionBuilder
    {
        private static MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        private static MethodInfo startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
        private static MethodInfo endsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });

        public static Expression<Func<T, bool>> GetExpression<T>(List<AppFilterItem2> filters)
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


        private static Expression GetExpression<T>(ParameterExpression param, AppFilterItem2 filter)
        {
            // The member you want to evaluate (x => x.FirstName)
            MemberExpression member = Expression.Property(param, filter.PropertyName);

            Type type = typeof(T).GetProperty($"{filter.PropertyName}")?.PropertyType!;

            // The value you want to evaluate
            ConstantExpression constant = Expression.Constant(filter.Value);

            var filterValue = filter.Value;

            if (type == typeof(int))
            {
                var aa = 10;
            }

            object value = null;

            if (type == typeof(DateTime) || type == typeof(DateTime?) && filterValue != null)
            {
                if (DateTime.TryParse(filterValue.ToString(), out DateTime date))
                {
                    value = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
                }
            }
            else if ((type != typeof(int) && type != typeof(List<int>) && type != typeof(int?)) || filter.Operator != Operator.Equals)
            {
                value = Convert.ToInt32(filterValue);
            }

            // Determine how we want to apply the expression
            switch (filter.Operator)
            {
                case Operator.Equals:
                    if (type == typeof(int))
                    {
                        value = Convert.ToInt32(filterValue);
                        return Expression.Equal(member, Expression.Constant(value));
                    }
                    return Expression.Equal(member, constant);

                case Operator.NotEqual:
                    return Expression.NotEqual(member, constant);

                case Operator.IsFalse:
                    return Expression.Equal(member, Expression.Constant(false));

                case Operator.IsTrue:
                    return Expression.Equal(member, Expression.Constant(true));

                case Operator.Contains:
                    return Expression.Call(member, containsMethod, constant);

                case Operator.GreaterThan:
                    return Expression.GreaterThan(member, constant);

                case Operator.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(member, constant);

                case Operator.LessThan:
                    return Expression.LessThan(member, constant);

                case Operator.LessThanOrEqualTo:
                    return Expression.LessThanOrEqual(member, constant);

                case Operator.StartsWith:
                    return Expression.Call(member, startsWithMethod, constant);

                case Operator.EndsWith:
                    return Expression.Call(member, endsWithMethod, constant);
            }
            return null;
        }

        private static BinaryExpression GetExpression<T>(ParameterExpression param, AppFilterItem2 filter1, AppFilterItem2 filter2)
        {
            Expression result1 = GetExpression<T>(param, filter1);
            Expression result2 = GetExpression<T>(param, filter2);
            return Expression.AndAlso(result1, result2);
        }
    }
}
