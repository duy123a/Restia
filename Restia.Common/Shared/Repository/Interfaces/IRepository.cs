using System.Linq.Expressions;

namespace Restia.Common.Shared.Repository.Interfaces
{
    public interface IRepository<T> where T : class
    {
        ValueTask<T?> GetByIdAsync(object id);

        Task<List<T>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<List<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            T entity,
            CancellationToken cancellationToken = default);

        Task AddRangeAsync(
            IEnumerable<T> entities,
            CancellationToken cancellationToken = default);

        void Update(T entity);

        void Delete(T entity);
    }
}
