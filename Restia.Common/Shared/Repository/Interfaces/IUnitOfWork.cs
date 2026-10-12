namespace Restia.Common.Shared.Repository.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveAsync(
            CancellationToken cancellationToken = default);
    }
}
