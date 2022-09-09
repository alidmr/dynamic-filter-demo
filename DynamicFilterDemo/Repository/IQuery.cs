namespace DynamicFilterDemo.Repository
{
    public interface IQuery<T>
    {
        IQueryable<T> Query();
    }
}
