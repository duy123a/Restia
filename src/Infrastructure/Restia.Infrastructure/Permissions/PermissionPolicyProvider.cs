using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Restia.Shared.Authorization;

namespace Restia.Infrastructure.Permissions;
internal class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="options">The options</param>
	public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
	{
		FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
	}

	/// <summary>
	/// Get policy async
	/// </summary>
	/// <param name="policyName">The policy name</param>
	/// <returns>A <see cref="Task"/> result is <see cref="AuthorizationPolicy"/>.</returns>
	public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
	{
		if (policyName.StartsWith(RestiaClaims.Permission, StringComparison.OrdinalIgnoreCase))
		{
			var policy = new AuthorizationPolicyBuilder();
			policy.AddRequirements(new PermissionRequirement(policyName));
			return Task.FromResult<AuthorizationPolicy?>(policy.Build());
		}

		return FallbackPolicyProvider.GetPolicyAsync(policyName);
	}

	/// <summary>
	/// Get Default Policy Async
	/// </summary>
	/// <returns>A <see cref="Task"/> result is <see cref="AuthorizationPolicy"/>.</returns>
	public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

	/// <summary>
	/// Get fallback policy async
	/// </summary>
	/// <returns>A <see cref="Task"/> result is <see cref="AuthorizationPolicy"/>.</returns>
	public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => Task.FromResult<AuthorizationPolicy?>(null);

	/// <summary>Get fallback policy provider</summary>
	public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }
}
