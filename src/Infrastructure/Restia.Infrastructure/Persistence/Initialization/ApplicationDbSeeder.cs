using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Restia.Infrastructure.Identity.Models;
using Restia.Infrastructure.Multitenancy.Models;

namespace Restia.Infrastructure.Persistence.Initialization;

internal class ApplicationDbSeeder
{
	/// <summary>The current tenant</summary>
	private readonly RestiaTenantInfo _currentTenant;
	/// <summary>The role manager</summary>
	private readonly RoleManager<ApplicationRole> _roleManager;
	/// <summary>The user manager</summary>
	private readonly UserManager<ApplicationUser> _userManager;
	/// <summary>The seeder runner</summary>
	private readonly CustomSeederRunner _seederRunner;
	/// <summary>The logger</summary>
	private readonly ILogger<ApplicationDbSeeder> _logger;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="currentMultiTenantContextAccessor">The current multi tenant context accessor</param>
	/// <param name="roleManager">The role manager</param>
	/// <param name="userManager">The user manager</param>
	/// <param name="seederRunner">The seeder runner</param>
	/// <param name="logger">The logger</param>
	public ApplicationDbSeeder(
		IMultiTenantContextAccessor<RestiaTenantInfo> currentMultiTenantContextAccessor,
		RoleManager<ApplicationRole> roleManager,
		UserManager<ApplicationUser> userManager,
		CustomSeederRunner seederRunner,
		ILogger<ApplicationDbSeeder> logger)
	{
		_currentTenant = currentMultiTenantContextAccessor.MultiTenantContext.TenantInfo!;
		_roleManager = roleManager;
		_userManager = userManager;
		_seederRunner = seederRunner;
		_logger = logger;
	}
}
