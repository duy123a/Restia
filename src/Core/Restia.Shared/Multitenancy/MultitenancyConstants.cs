namespace Restia.Shared.Multitenancy;

public class MultitenancyConstants
{
	/// <summary>
	/// Root
	/// </summary>
	public static class Root
	{
		/// <summary>The id</summary>
		public const string Id = "root";
		/// <summary>The name</summary>
		public const string Name = "Root";
		/// <summary>The email address</summary>
		public const string EmailAddress = "admin@root.com";
	}

	/// <summary>Default password</summary>
	public const string DefaultPassword = "123Pa$$word!";
	/// <summary>Tenant id name</summary>
	public const string TenantIdName = "tenant";
}
