using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Restia.Infrastructure.Multitenancy.Services;

/// <summary>
/// Tenant Health Check
/// </summary>
public class TenantHealthCheck : IHealthCheck
{
	/// <summary>
	/// Check health async
	/// </summary>
	/// <param name="context">The context</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>A <see cref="Task"/> result is <see cref="HealthCheckResult"/>.</returns>
	public Task<HealthCheckResult> CheckHealthAsync(
		HealthCheckContext context,
		CancellationToken cancellationToken = default)
	{
		// Perform custom checks here
		// TODO: Should check connecting status to correct tenant DB maybe?
		var healthCheckResultHealthy = true;

		if (healthCheckResultHealthy)
		{
			return Task.FromResult(HealthCheckResult.Healthy("The check indicates a healthy result."));
		}

		return Task.FromResult(HealthCheckResult.Unhealthy("The check indicates an unhealthy result."));
	}
}
