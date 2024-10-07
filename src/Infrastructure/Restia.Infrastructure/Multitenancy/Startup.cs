using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Restia.Infrastructure.Multitenancy.Context;
using Restia.Infrastructure.Multitenancy.Models;
using Restia.Infrastructure.Persistence;
using Restia.Infrastructure.Persistence.Settings;
using Restia.Shared.Authorization;
using Restia.Shared.Multitenancy;

namespace Restia.Infrastructure.Multitenancy;

internal static class Startup
{
	/// <summary>
	/// Add multitenancy
	/// </summary>
	/// <param name="services">The services</param>
	/// <returns>A <see cref="IServiceCollection"/>.</returns>
	internal static IServiceCollection AddMultitenancy(this IServiceCollection services)
	{
		return services
			.AddDbContext<TenantDbContext>((provider, options) =>
			{
				// TODO: We should probably add specific dbprovider/connectionstring setting for the tenantDb with a fallback to the main databasesettings
				var databaseSettings = provider.GetRequiredService<IOptions<DatabaseSettings>>().Value;
				options.UseDatabase(databaseSettings.DBProvider, databaseSettings.ConnectionString);
			})
			// This will register IMultiTenantContextAccessor<RestiaTenantInfo> to DI which you can use to take RestiaTenantInfo
			// The RestiaTenantInfo you get will be dynamic resolved based on various strategy (only id)
			.AddMultiTenant<RestiaTenantInfo>()
				.WithClaimStrategy(RestiaClaims.Tenant)
				.WithHeaderStrategy(MultitenancyConstants.TenantIdName)
				.WithQueryStringStrategy(MultitenancyConstants.TenantIdName)
				// The rest of tenant info data can be retrieve from the database
				.WithEFCoreStore<TenantDbContext, RestiaTenantInfo>()
				.Services;
	}

	/// <summary>
	/// Use multiTenancy
	/// </summary>
	/// <param name="app">The application builder</param>
	/// <returns></returns>
	internal static IApplicationBuilder UseMultiTenancy(this IApplicationBuilder app) =>
		app.UseMultiTenant();

	/// <summary>
	/// With query string strategy
	/// </summary>
	/// <param name="builder">The builder</param>
	/// <param name="queryStringKey">The query string key</param>
	/// <returns>A builder as <see cref="RestiaTenantInfo"/>.</returns>
	private static MultiTenantBuilder<RestiaTenantInfo> WithQueryStringStrategy(
		this MultiTenantBuilder<RestiaTenantInfo> builder,
		string queryStringKey)
	{
		return builder.WithDelegateStrategy(context =>
		{
			if (context is not HttpContext httpContext)
			{
				return Task.FromResult((string?)null);
			}

			httpContext.Request.Query.TryGetValue(queryStringKey, out var tenantIdParam);

			return Task.FromResult((string?)tenantIdParam.ToString());
		});
	}
}
