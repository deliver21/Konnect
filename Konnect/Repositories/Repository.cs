using Konnect.Data;
using Microsoft.EntityFrameworkCore;

namespace Konnect.Repository
{
    public class Repository<T> : IRepository<T> where T:class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet { get; set; }
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }
        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }
        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }
        public T Get(System.Linq.Expressions.Expression<Func<T, bool>>? filter = null, bool track = false) 
        {
            IQueryable<T> query;
            if(track)
            {
                query = dbSet;
            }
            else
            {
                query = dbSet.AsNoTracking();
            }
            query = query.Where(filter);

            return query.FirstOrDefault();
        }
        public IEnumerable<T> GetAll(System.Linq.Expressions.Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query;
            query = dbSet.AsNoTracking();
            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.ToList();
        }
    }
}
