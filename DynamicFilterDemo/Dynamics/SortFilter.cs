namespace DynamicFilterDemo.Dynamics
{
    public class SortFilter
    {
        public string Field { get; set; }
        public string Dir { get; set; }

        public SortFilter()
        {
        }

        public SortFilter(string field, string dir)
        {
            Field = field;
            Dir = dir;
        }
    }
}
