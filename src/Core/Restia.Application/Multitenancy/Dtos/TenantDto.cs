namespace Restia.Application.Multitenancy.Dtos;

public class TenantDto
{
	/// <summary>Get and set id</summary>
	public string Id { get; set; } = default!;
	/// <summary>Get and set name</summary>
	public string Name { get; set; } = default!;
	/// <summary>Get and set connection string</summary>
	public string? ConnectionString { get; set; }
	/// <summary>Get and set admin email</summary>
	public string AdminEmail { get; set; } = default!;
	/// <summary>Get and set is active</summary>
	public bool IsActive { get; set; }
	/// <summary>Get and set valid upto</summary>
	public DateTime ValidUpto { get; set; }
	/// <summary>Get and set issuer</summary>
	public string? Issuer { get; set; }
}
