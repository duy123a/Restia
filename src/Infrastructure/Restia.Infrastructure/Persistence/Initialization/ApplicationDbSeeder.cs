using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restia.Infrastructure.Identity.Models;
using Restia.Infrastructure.Multitenancy.Models;
using Restia.Infrastructure.Persistence.Context;
using Restia.Shared.Authorization;
using Restia.Shared.Multitenancy;

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

	/// <summary>
	/// Seed database async
	/// </summary>
	/// <param name="dbContext">The Db context</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>A task</returns>
	public async Task SeedDatabaseAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
	{
		await SeedRolesAsync(dbContext);
		await SeedAdminUserAsync();
		await _seederRunner.RunSeedersAsync(cancellationToken);
	}

	/// <summary>
	/// Seed roles async
	/// </summary>
	/// <param name="dbContext">The Db context</param>
	/// <returns>A task</returns>
	private async Task SeedRolesAsync(ApplicationDbContext dbContext)
	{
		foreach (var roleName in RestiaRoles.DefaultRoles)
		{
			if (await _roleManager.Roles.SingleOrDefaultAsync(r => r.Name == roleName)
				is not ApplicationRole role)
			{
				// Create the role
				_logger.LogInformation("Seeding {role} Role for '{tenantId}' Tenant.", roleName, _currentTenant.Id);
				role = new ApplicationRole(roleName, $"{roleName} Role for {_currentTenant.Id} Tenant");
				await _roleManager.CreateAsync(role);
			}

			// Assign permissions
			if (roleName == RestiaRoles.Basic)
			{
				await AssignPermissionsToRoleAsync(dbContext, RestiaPermissions.Basic, role);
			}
			else if (roleName == RestiaRoles.Admin)
			{
				await AssignPermissionsToRoleAsync(dbContext, RestiaPermissions.Admin, role);

				if (_currentTenant.Id == MultitenancyConstants.Root.Id)
				{
					await AssignPermissionsToRoleAsync(dbContext, RestiaPermissions.Root, role);
				}
			}
		}
	}

	/// <summary>
	/// Assign permissions to role async
	/// </summary>
	/// <param name="dbContext">The db context</param>
	/// <param name="permissions">The permissions</param>
	/// <param name="role">The role</param>
	/// <returns>A task</returns>
	private async Task AssignPermissionsToRoleAsync(
		ApplicationDbContext dbContext,
		IReadOnlyList<RestiaPermission> permissions,
		ApplicationRole role)
	{
		var currentClaims = await _roleManager.GetClaimsAsync(role);
		foreach (var permission in permissions)
		{
			if (!currentClaims.Any(c => (c.Type == RestiaClaims.Permission) && (c.Value == permission.Name)))
			{
				_logger.LogInformation(
					"Seeding {role} Permission '{permission}' for '{tenantId}' Tenant.",
					role.Name,
					permission.Name,
					_currentTenant.Id);
				dbContext.RoleClaims.Add(new ApplicationRoleClaim
				{
					RoleId = role.Id,
					ClaimType = RestiaClaims.Permission,
					ClaimValue = permission.Name,
					CreatedBy = "ApplicationDbSeeder",
					CreatedOn = DateTime.UtcNow,
				});
				await dbContext.SaveChangesAsync();
			}
		}
	}

	/// <summary>
	/// Seed admin user async
	/// </summary>
	/// <returns>A task</returns>
	private async Task SeedAdminUserAsync()
	{
		if (string.IsNullOrWhiteSpace(_currentTenant.Id)
			|| string.IsNullOrWhiteSpace(_currentTenant.AdminEmail))
		{
			return;
		}

		if (await _userManager.Users.FirstOrDefaultAsync(u => u.Email == _currentTenant.AdminEmail)
			is not ApplicationUser adminUser)
		{
			string adminUserName = $"{_currentTenant.Id.Trim()}.{RestiaRoles.Admin}".ToLowerInvariant();
			adminUser = new ApplicationUser
			{
				FirstName = _currentTenant.Id.Trim().ToLowerInvariant(),
				LastName = RestiaRoles.Admin,
				Email = _currentTenant.AdminEmail,
				UserName = adminUserName,
				EmailConfirmed = true,
				PhoneNumberConfirmed = true,
				NormalizedEmail = _currentTenant.AdminEmail?.ToUpperInvariant(),
				NormalizedUserName = adminUserName.ToUpperInvariant(),
				IsActive = true
			};

			_logger.LogInformation("Seeding Default Admin User for '{tenantId}' Tenant.", _currentTenant.Id);
			var password = new PasswordHasher<ApplicationUser>();
			adminUser.PasswordHash = password.HashPassword(adminUser, MultitenancyConstants.DefaultPassword);
			await _userManager.CreateAsync(adminUser);
		}

		// Assign role to user
		if (!await _userManager.IsInRoleAsync(adminUser, RestiaRoles.Admin))
		{
			_logger.LogInformation("Assigning Admin Role to Admin User for '{tenantId}' Tenant.", _currentTenant.Id);
			await _userManager.AddToRoleAsync(adminUser, RestiaRoles.Admin);
		}
	}
}
