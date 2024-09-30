using MediatR;
using Restia.Application.Common.Validation;
using Restia.Application.Multitenancy.ServiceInterfaces;

namespace Restia.Application.Multitenancy.Features.CreateTenant;

public class CreateTenantRequest : IRequest<string>
{
	/// <summary>Get and set tenant id</summary>
	public string Id { get; set; } = default!;
	/// <summary>Get and set tenant name</summary>
	public string Name { get; set; } = default!;
	/// <summary>Get and set connection string</summary>
	public string? ConnectionString { get; set; }
	/// <summary>Get and set admin email</summary>
	public string AdminEmail { get; set; } = default!;
}

public class CreateTenantRequestValidator : CustomValidator<CreateTenantRequest>
{

}

public class CreateTenantRequestHandler : IRequestHandler<CreateTenantRequest, string>
{
	/// <summary>The tenant service</summary>
	private readonly ITenantService _tenantService;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="tenantService">The tenant service</param>
	public CreateTenantRequestHandler(ITenantService tenantService) => _tenantService = tenantService;

	/// <summary>
	/// Handle process
	/// </summary>
	/// <param name="request">The request</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>A <see cref="Task" /> result is string.</returns>
	public Task<string> Handle(CreateTenantRequest request, CancellationToken cancellationToken) =>
		_tenantService.CreateAsync(request, cancellationToken);
}
