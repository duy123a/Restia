using Finbuckle.MultiTenant.Abstractions;
using Mapster;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Restia.Application.Common.Exceptions;
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


	/// <summary>
	/// Get all async
	/// </summary>
	/// <returns>A <see cref="Task"/> results in the list of <see cref="TenantDto"/>.</returns>
	public async Task<List<TenantDto>> GetAllAsync()
	{
		var tenants = (await _tenantStore.GetAllAsync()).Adapt<List<TenantDto>>();
		tenants.ForEach(t => t.ConnectionString = _csSecurer.MakeSecure(t.ConnectionString));
		return tenants;
	}

	/// <summary>
	/// Exists with id async
	/// </summary>
	/// <param name="id">The tenant id</param>
	/// <returns>A <see cref="Task" /> result is a boolean: True if the tenant has the same id.</returns>
	public async Task<bool> ExistsWithIdAsync(string id) =>
		await _tenantStore.TryGetAsync(id) is not null;

	/// <summary>
	/// Exists with name async
	/// </summary>
	/// <param name="name">The tenant name</param>
	/// <returns>A <see cref="Task" /> result is a boolean: True if the tenant has the same name.</returns>
	public async Task<bool> ExistsWithNameAsync(string name) =>
		(await _tenantStore.GetAllAsync()).Any(t => t.Name == name);

	/// <summary>
	/// Get by id async
	/// </summary>
	/// <param name="id">The tenant id</param>
	/// <returns>A <see cref="Task"/> result is a <see cref="TenantDto"/>.</returns>
	public async Task<TenantDto> GetByIdAsync(string id) =>
		(await GetTenantInfoAsync(id))
			.Adapt<TenantDto>();

	/// <summary>
	/// Create async
	/// </summary>
	/// <param name="request">The request</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>A <see cref="Task" /> result is a string.</returns>
	public async Task<string> CreateAsync(CreateTenantRequest request, CancellationToken cancellationToken)
	{
		if (request.ConnectionString?.Trim() == _dbSettings.ConnectionString.Trim())
			request.ConnectionString = string.Empty;

		var tenant = new RestiaTenantInfo(
			request.Id,
			request.Name,
			request.ConnectionString,
			request.AdminEmail);
		await _tenantStore.TryAddAsync(tenant);

		// TODO: run this in a hangfire job? will then have to send mail when it's ready or not
		try
		{
			await _dbInitializer.InitializeApplicationDbForTenantAsync(tenant, cancellationToken);
		}
		catch
		{
			await _tenantStore.TryRemoveAsync(request.Id);
			throw;
		}

		return tenant.Id;
	}

	/// <summary>
	/// Activate async
	/// </summary>
	/// <param name="id">The tenant id</param>
	/// <returns>A <see cref="Task" /> result is a string.</returns>
	public async Task<string> ActivateAsync(string id)
	{
		var tenant = await GetTenantInfoAsync(id);

		if (tenant.IsActive)
		{
			throw new ConflictException(_localizer["Tenant is already activated."]);
		}

		tenant.Activate();
		await _tenantStore.TryUpdateAsync(tenant);
		return _localizer["Tenant {0} is now Activated.", id];
	}

	/// <summary>
	/// Deactivate async
	/// </summary>
	/// <param name="id">The tenant id</param>
	/// <returns>A <see cref="Task" /> result is a string.</returns>
	public async Task<string> DeactivateAsync(string id)
	{
		var tenant = await GetTenantInfoAsync(id);
		if (!tenant.IsActive)
		{
			throw new ConflictException(_localizer["Tenant is already deactivated."]);
		}

		tenant.Deactivate();
		await _tenantStore.TryUpdateAsync(tenant);
		return _localizer["Tenant {0} is now Deactivated.", id];
	}

	/// <summary>
	/// Update subscription async
	/// </summary>
	/// <param name="id">The tenant id</param>
	/// <param name="extendedExpiryDate">The extended expiry date</param>
	/// <returns>A <see cref="Task" /> result is a string.</returns>
	public async Task<string> UpdateSubscriptionAsync(string id, DateTime extendedExpiryDate)
	{
		var tenant = await GetTenantInfoAsync(id);
		tenant.SetValidity(extendedExpiryDate);
		await _tenantStore.TryUpdateAsync(tenant);
		return _localizer["Tenant {0}'s Subscription Upgraded. Now Valid till {1}.", id, tenant.ValidUpto];
	}

	/// <summary>
	/// Get Tenant information async
	/// </summary>
	/// <param name="id">The id</param>
	/// <returns>A <see cref="Task"/> result is a <see cref="W2TenantInfo"/>.</returns>
	/// <exception cref="NotFoundException">A NotFoundException</exception>
	private async Task<RestiaTenantInfo> GetTenantInfoAsync(string id) =>
		await _tenantStore.TryGetAsync(id)
			?? throw new NotFoundException(_localizer["{0} {1} Not Found.", typeof(RestiaTenantInfo).Name, id]);
}
