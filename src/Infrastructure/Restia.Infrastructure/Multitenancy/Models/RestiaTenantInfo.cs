using Finbuckle.MultiTenant.Abstractions;
using Restia.Shared.Multitenancy;

namespace Restia.Infrastructure.Multitenancy.Models;
public class RestiaTenantInfo : ITenantInfo
{
	/// <summary>
	/// Default contructor
	/// </summary>
	public RestiaTenantInfo()
	{
	}

	public RestiaTenantInfo(
		string id,
		string name,
		string? connectionString,
		string adminEmail)
	{
		Id = id;
		Identifier = id;
		Name = name;
		ConnectionString = connectionString ?? string.Empty;
		AdminEmail = adminEmail;
		IsActive = true;

		// Add Default 1 Month Validity for all new tenants. Something like a DEMO period for tenants.
		ValidUpto = DateTime.UtcNow.AddMonths(1);
	}

	/// <summary>
	/// Add validity
	/// </summary>
	/// <param name="months">Th months</param>
	public void AddValidity(int months) =>
		ValidUpto = ValidUpto.AddMonths(months);

	/// <summary>
	/// Set validity
	/// </summary>
	/// <param name="validTill">The valid till</param>
	/// <exception cref="Exception">A Exception</exception>
	public void SetValidity(in DateTime validTill)
	{
		ValidUpto = ValidUpto < validTill
			? validTill
			: throw new Exception("Subscription cannot be backdated.");
	}

	/// <summary>
	/// Activate
	/// </summary>
	/// <exception cref="InvalidOperationException">A InvalidOperationException</exception>
	public void Activate()
	{
		if (Id == MultitenancyConstants.Root.Id)
		{
			throw new InvalidOperationException("Invalid Tenant");
		}

		IsActive = true;
	}

	/// <summary>
	/// Deactivate
	/// </summary>
	/// <exception cref="InvalidOperationException">A InvalidOperationException</exception>
	public void Deactivate()
	{
		if (Id == MultitenancyConstants.Root.Id)
		{
			throw new InvalidOperationException("Invalid Tenant");
		}

		IsActive = false;
	}

	/// <summary>The actual TenantId, which is also used in the TenantId shadow property on the multitenant entities.</summary>
	public string Id { get; set; } = default!;
	/// <summary>The identifier that is used in headers/routes/querystrings. This is set to the same as Id to avoid confusion.</summary>
	public string Identifier { get; set; } = default!;
	/// <summary>The tenant name</summary>
	public string Name { get; set; } = default!;
	/// <summary>The tenant connection string</summary>
	public string ConnectionString { get; set; } = default!;
	/// <summary>The tenant admin email</summary>
	public string AdminEmail { get; private set; } = default!;
	/// <summary>Is active</summary>
	public bool IsActive { get; private set; }
	/// <summary>The valid upto</summary>
	public DateTime ValidUpto { get; private set; }

	/// <summary>Tenant information: Id</summary>
	string? ITenantInfo.Id
	{
		get => Id;
		set => Id = value ?? throw new InvalidOperationException("Id can't be null.");
	}

	/// <summary>Tenant information: identifier</summary>
	string? ITenantInfo.Identifier
	{
		get => Identifier;
		set => Identifier = value ?? throw new InvalidOperationException("Identifier can't be null.");
	}

	/// <summary>Tenant information: name</summary>
	string? ITenantInfo.Name
	{
		get => Name;
		set => Name = value ?? throw new InvalidOperationException("Name can't be null.");
	}
}
