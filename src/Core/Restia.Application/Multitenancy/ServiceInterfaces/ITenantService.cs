using Restia.Application.Multitenancy.Dtos;
using Restia.Application.Multitenancy.Features.CreateTenant;

namespace Restia.Application.Multitenancy.ServiceInterfaces;

public interface ITenantService
{
	/// <summary>
	/// Get all async
	/// </summary>
	/// <returns>A <see cref="Task"/> results in the list of <see cref="TenantDto"/>.</returns>
	Task<List<TenantDto>> GetAllAsync();

	/// <summary>
	/// Exists with id async
	/// </summary>
	/// <param name="id">The tenant id</param>
	/// <returns>A <see cref="Task" /> result is a boolean: True if the tenant has the same id.</returns>
	Task<bool> ExistsWithIdAsync(string id);

	/// <summary>
	/// Exists with name async
	/// </summary>
	/// <param name="name">The tenant name</param>
	/// <returns>A <see cref="Task" /> result is a boolean: True if the tenant has the same name.</returns>
	Task<bool> ExistsWithNameAsync(string name);

	/// <summary>
	/// Get by id async
	/// </summary>
	/// <param name="id">The tenant id</param>
	/// <returns>A <see cref="Task"/> result is a <see cref="TenantDto"/>.</returns>
	Task<TenantDto> GetByIdAsync(string id);

	/// <summary>
	/// Create async
	/// </summary>
	/// <param name="request">The request</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>A <see cref="Task" /> result is a string.</returns>
	Task<string> CreateAsync(CreateTenantRequest request, CancellationToken cancellationToken);

	/// <summary>
	/// Activate async
	/// </summary>
	/// <param name="id">The tenant id</param>
	/// <returns>A <see cref="Task" /> result is a string.</returns>
	Task<string> ActivateAsync(string id);

	/// <summary>
	/// Deactivate async
	/// </summary>
	/// <param name="id">The tenant id</param>
	/// <returns>A <see cref="Task" /> result is a string.</returns>
	Task<string> DeactivateAsync(string id);

	/// <summary>
	/// Update subscription async
	/// </summary>
	/// <param name="id">The tenant id</param>
	/// <param name="extendedExpiryDate">The extended expiry date</param>
	/// <returns>A <see cref="Task" /> result is a string.</returns>
	Task<string> UpdateSubscriptionAsync(string id, DateTime extendedExpiryDate);
}
