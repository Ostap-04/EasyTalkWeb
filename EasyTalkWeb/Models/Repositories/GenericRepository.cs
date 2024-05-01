using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using EasyTalkWeb.Persistance;

namespace EasyTalkWeb.Models.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public readonly AppDbContext _context;

        public DbSet<T> _table { get; }

        public GenericRepository(AppDbContext _context)
        {
            this._context = _context;
            _table = _context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _table.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            return await _table.FindAsync(id);
        }

        public virtual async Task<T?> GetOneAsync(Expression<Func<T, bool>> filter)
        {
            return await _table.FirstOrDefaultAsync(filter);
        }

        public virtual async Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includeProperties = "")
        {
            IQueryable<T> query = _table;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }

        public virtual async Task AddAsync(T obj)
        {
            await _table.AddAsync(obj);
            await SaveAsync();
        }

        public virtual async Task Update(T obj)
        {
            _table.Attach(obj);
            _context.Entry(obj).State = EntityState.Modified;
            await SaveAsync();
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            T existing = await _table.FindAsync(id);
            if (existing != null)
            {
                _table.Remove(existing);
            }

            await SaveAsync();
        }

        public virtual async Task Delete(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _table.Attach(entity);
            }
            _table.Remove(entity);

            await SaveAsync();
        }

        public virtual async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public virtual void Detach(T entity)
        {
            var entry = _context.Entry(entity);
            if (entry.State != EntityState.Detached)
            {
                entry.State = EntityState.Detached;
            }
        }
    }
}
