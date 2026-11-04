using Microsoft.EntityFrameworkCore.Storage;

namespace Restia.Common.Shared.Repository.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default);

        Task<IDbContextTransaction> BeginTransactionAsync(
            CancellationToken cancellationToken = default);

        IExecutionStrategy CreateExecutionStrategy();
    }
}
