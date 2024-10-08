using System.Collections.ObjectModel;

namespace Restia.Shared.Authorization;

public static class RestiaPermissions
{
	/// <summary>Permissions: All</summary>
	private static readonly RestiaPermission[] _all =
	[
		new("View Dashboard", RestiaActions.View, RestiaResources.Dashboard),
		new("View Hangfire", RestiaActions.View, RestiaResources.Hangfire),
		new("View Users", RestiaActions.View, RestiaResources.Users),
		new("Search Users", RestiaActions.Search, RestiaResources.Users),
		new("Create Users", RestiaActions.Create, RestiaResources.Users),
		new("Update Users", RestiaActions.Update, RestiaResources.Users),
		new("Delete Users", RestiaActions.Delete, RestiaResources.Users),
		new("Export Users", RestiaActions.Export, RestiaResources.Users),
		new("View UserRoles", RestiaActions.View, RestiaResources.UserRoles),
		new("Update UserRoles", RestiaActions.Update, RestiaResources.UserRoles),
		new("View Roles", RestiaActions.View, RestiaResources.Roles),
		new("Create Roles", RestiaActions.Create, RestiaResources.Roles),
		new("Update Roles", RestiaActions.Update, RestiaResources.Roles),
		new("Delete Roles", RestiaActions.Delete, RestiaResources.Roles),
		new("View RoleClaims", RestiaActions.View, RestiaResources.RoleClaims),
		new("Update RoleClaims", RestiaActions.Update, RestiaResources.RoleClaims),
		new("View Products", RestiaActions.View, RestiaResources.Products, IsBasic: true),
		new("Search Products", RestiaActions.Search, RestiaResources.Products, IsBasic: true),
		new("Create Products", RestiaActions.Create, RestiaResources.Products),
		new("Update Products", RestiaActions.Update, RestiaResources.Products),
		new("Delete Products", RestiaActions.Delete, RestiaResources.Products),
		new("Export Products", RestiaActions.Export, RestiaResources.Products),
		new("View Brands", RestiaActions.View, RestiaResources.Brands, IsBasic: true),
		new("Search Brands", RestiaActions.Search, RestiaResources.Brands, IsBasic: true),
		new("Create Brands", RestiaActions.Create, RestiaResources.Brands),
		new("Update Brands", RestiaActions.Update, RestiaResources.Brands),
		new("Delete Brands", RestiaActions.Delete, RestiaResources.Brands),
		new("Generate Brands", RestiaActions.Generate, RestiaResources.Brands),
		new("Clean Brands", RestiaActions.Clean, RestiaResources.Brands),
		new("View Tenants", RestiaActions.View, RestiaResources.Tenants, IsRoot: true),
		new("Create Tenants", RestiaActions.Create, RestiaResources.Tenants, IsRoot: true),
		new("Update Tenants", RestiaActions.Update, RestiaResources.Tenants, IsRoot: true),
		new("Upgrade Tenant Subscription", RestiaActions.UpgradeSubscription, RestiaResources.Tenants, IsRoot: true),
		new("View Coupons", RestiaActions.View, RestiaResources.Coupons, IsBasic: true),
		new("Search Coupons", RestiaActions.Search, RestiaResources.Coupons, IsBasic: true),
		new("Export Coupons", RestiaActions.Export, RestiaResources.Coupons)
	];

	/// <summary>
	/// Get all permissions
	/// </summary>
	public static IReadOnlyList<RestiaPermission> All { get; } = new ReadOnlyCollection<RestiaPermission>(_all);

	/// <summary>
	/// Get root permissions
	/// </summary>
	public static IReadOnlyList<RestiaPermission> Root { get; } = new ReadOnlyCollection<RestiaPermission>(
		_all.Where(p => p.IsRoot).ToArray());

	/// <summary>
	/// Get admin permissions
	/// </summary>
	public static IReadOnlyList<RestiaPermission> Admin { get; } = new ReadOnlyCollection<RestiaPermission>(
		_all.Where(p => !p.IsRoot).ToArray());

	/// <summary>
	/// Get basic permissions
	/// </summary>
	public static IReadOnlyList<RestiaPermission> Basic { get; } = new ReadOnlyCollection<RestiaPermission>(
		_all.Where(p => p.IsBasic).ToArray());
}

public record RestiaPermission(
	string Description,
	string Action,
	string Resource,
	bool IsBasic = false,
	bool IsRoot = false)
{
	/// <summary>
	/// Get name
	/// </summary>
	public string Name => NameFor(Action, Resource);

	/// <summary>
	/// Create and get a name by action and resource
	/// </summary>
	/// <param name="action">The action</param>
	/// <param name="resource">The resource</param>
	/// <returns>A name</returns>
	public static string NameFor(string action, string resource)
		=> $"Permissions.{resource}.{action}";
}
