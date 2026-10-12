using Microsoft.EntityFrameworkCore;
using Restia.Common.Data.Context;
using Restia.Common.Shared.Repository.Interfaces;
using System.Linq.Expressions;

namespace Restia.Common.Shared.Repository
{
    public class BaseRepository<T> : IRepository<T>
    where T : class
    {
        protected readonly AppDbContext Context;
        protected readonly DbSet<T> DbSet;

        public BaseRepository(AppDbContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        public virtual ValueTask<T?> GetByIdAsync(object id)
        {
            return DbSet.FindAsync(id);
        }

        public virtual async Task<List<T>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await DbSet.ToListAsync(cancellationToken);
        }

        public virtual async Task<List<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(predicate)
                .ToListAsync(cancellationToken);
        }

        public virtual async Task AddAsync(
            T entity,
            CancellationToken cancellationToken = default)
        {
            await DbSet.AddAsync(entity, cancellationToken);
        }

        public virtual async Task AddRangeAsync(
            IEnumerable<T> entities,
            CancellationToken cancellationToken = default)
        {
            await DbSet.AddRangeAsync(entities, cancellationToken);
        }

        public virtual void Update(T entity)
        {
            DbSet.Update(entity);
        }

        public virtual void Delete(T entity)
        {
            DbSet.Remove(entity);
        }
    }
}
