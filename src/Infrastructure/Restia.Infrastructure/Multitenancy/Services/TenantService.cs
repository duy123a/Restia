using Restia.Application.Multitenancy.Dtos;
using Restia.Application.Multitenancy.Features.CreateTenant;
using Restia.Application.Multitenancy.ServiceInterfaces;

namespace Restia.Infrastructure.Multitenancy.Services;

internal class TenantService : ITenantService
{
	public Task<string> ActivateAsync(string id)
	{
		throw new NotImplementedException();
	}

	public Task<string> CreateAsync(CreateTenantRequest request, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task<string> DeactivateAsync(string id)
	{
		throw new NotImplementedException();
	}

	public Task<bool> ExistsWithIdAsync(string id)
	{
		throw new NotImplementedException();
	}

	public Task<bool> ExistsWithNameAsync(string name)
	{
		throw new NotImplementedException();
	}

	public Task<List<TenantDto>> GetAllAsync()
	{
		throw new NotImplementedException();
	}

	public Task<TenantDto> GetByIdAsync(string id)
	{
		throw new NotImplementedException();
	}

	public Task<string> UpdateSubscriptionAsync(string id, DateTime extendedExpiryDate)
	{
		throw new NotImplementedException();
	}
}
