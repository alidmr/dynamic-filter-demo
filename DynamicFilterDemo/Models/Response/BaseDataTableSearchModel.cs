using DynamicFilterDemo.Filters;

namespace DynamicFilterDemo.Models.Response
{
    public class BaseDataTableSearchModel
    {
        public int Start { get; set; }

        public int Length { get; set; }

        public int Draw { get; set; }

        public string Sort { get; set; }

        public string SortColumn { get; set; }

        public string SortBy { get; set; }

        public List<AppFilterItem> Parameters { get; set; }
    }
}
