namespace DynamicFilterDemo.Response
{
    public class MyDataResponse<T>
    {
        public List<T> Items { get; set; }
        public int Count { get; set; }
    }
}
