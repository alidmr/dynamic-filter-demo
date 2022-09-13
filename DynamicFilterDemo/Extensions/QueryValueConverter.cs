using Microsoft.AspNetCore.Authorization;

namespace DynamicFilterDemo.Extensions
{
    public static class QueryValueConverter
    {
        public static object GetValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            switch (value.ToUpperInvariant())
            {
                case "@ME":
                    return string.Empty;
                case "@TODAY":
                    return DateTime.Today;
                case "@NOW":
                    return DateTime.Now;
                case "@MONTH_START":
                    return new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                case "@COMPANIES":
                    return string.Empty;
                case "@INSTITUTION":
                    return string.Empty;
            }

            return value;
        }
    }
}
