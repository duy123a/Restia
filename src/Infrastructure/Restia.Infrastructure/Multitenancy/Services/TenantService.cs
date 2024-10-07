using Finbuckle.MultiTenant.Abstractions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Restia.Application.Common.Persistence;
using Restia.Application.Multitenancy.Dtos;
using Restia.Application.Multitenancy.Features.CreateTenant;
using Restia.Application.Multitenancy.ServiceInterfaces;
using Restia.Infrastructure.Multitenancy.Models;
using Restia.Infrastructure.Persistence.Initialization.Interfaces;
using Restia.Infrastructure.Persistence.Settings;

namespace Restia.Infrastructure.Multitenancy.Services;

internal class TenantService : ITenantService
{
	/// <summary>The tenant store</summary>
	private readonly IMultiTenantStore<RestiaTenantInfo> _tenantStore;
	/// <summary>The CS securer</summary>
	private readonly IConnectionStringSecurer _csSecurer;
	/// <summary>The db initializer</summary>
	private readonly IDatabaseInitializer _dbInitializer;
	/// <summary>The localizer</summary>
	private readonly IStringLocalizer _localizer;
	/// <summary>The db settings</summary>
	private readonly DatabaseSettings _dbSettings;

	public TenantService(
		IMultiTenantStore<RestiaTenantInfo> tenantStore,
		IConnectionStringSecurer csSecurer,
		IDatabaseInitializer dbInitializer,
		IStringLocalizer<TenantService> localizer,
		IOptions<DatabaseSettings> dbSettings)
	{
		_tenantStore = tenantStore;
		_csSecurer = csSecurer;
		_dbInitializer = dbInitializer;
		_localizer = localizer;
		_dbSettings = dbSettings.Value;
	}

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
