using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using Restia.Application.Common.Persistence;
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

/// <summary>
/// Create Tenant Request Validator
/// </summary>
public class CreateTenantRequestValidator : CustomValidator<CreateTenantRequest>
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="tenantService">The tenant service</param>
	/// <param name="T">Type of <see cref="IStringLocalizer"/>.</param>
	/// <param name="connectionStringValidator">The connection string validator</param>
	public CreateTenantRequestValidator(
		ITenantService tenantService,
		IStringLocalizer<CreateTenantRequestValidator> T,
		IConnectionStringValidator connectionStringValidator)
	{
		RuleFor(t => t.Id).Cascade(CascadeMode.Stop)
			.NotEmpty()
			.MustAsync(async (id, _) => !await tenantService.ExistsWithIdAsync(id))
				.WithMessage((_, id) => T["Tenant {0} already exist", id]);

		RuleFor(t => t.Name).Cascade(CascadeMode.Stop)
			.NotEmpty()
			.MustAsync(async (name, _) => !await tenantService.ExistsWithNameAsync(name!))
				.WithMessage((_, name) => T["Tenant {0} already exists.", name]);

		RuleFor(t => t.ConnectionString).Cascade(CascadeMode.Stop)
			.Must((_, cs) => string.IsNullOrWhiteSpace(cs) || connectionStringValidator.TryValidate(cs))
				.WithMessage(T["Connection string invalid."]);

		RuleFor(t => t.AdminEmail).Cascade(CascadeMode.Stop)
			.NotEmpty()
			.EmailAddress();
	}
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
