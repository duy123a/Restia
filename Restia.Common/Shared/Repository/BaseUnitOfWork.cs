using Restia.Common.Data.Context;
using Restia.Common.Shared.Repository.Interfaces;

namespace Restia.Common.Shared.Repository
{
    public class BaseUnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public BaseUnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public Task<int> SaveAsync(
            CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
