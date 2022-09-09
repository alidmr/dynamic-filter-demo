namespace DynamicFilterDemo.Dynamics
{
    public class DynamicFilter
    {
        public IEnumerable<SortFilter>? Sort { get; set; }
        public Filter? Filter { get; set; }

        public DynamicFilter()
        {
        }

        public DynamicFilter(IEnumerable<SortFilter>? sort, Filter? filter)
        {
            Sort = sort;
            Filter = filter;
        }
    }
}
