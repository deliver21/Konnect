namespace Konnect.Repository
{
    public interface IRepository<T> where T:class
    {
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        T Get(System.Linq.Expressions.Expression<Func<T, bool>>? filter = null , bool track = false);
        IEnumerable<T> GetAll(System.Linq.Expressions.Expression<Func<T, bool>>? filter = null);
    }
}
