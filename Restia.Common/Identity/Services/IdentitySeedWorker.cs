using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restia.Common.Identity.Entities;
using Restia.Common.Identity.Enums;
using Restia.Common.Tenant.Services.Interfaces;

namespace Restia.Common.Identity.Services
{
    public sealed class IdentitySeedWorker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public IdentitySeedWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var tenantService = scope.ServiceProvider.GetRequiredService<ITenantService>();
            await tenantService.InitializeAllTenantsAsync(cancellationToken);
        }

        public static async Task SeedRolesAsync(RoleManager<AppRole> roleManager)
        {
            await EnsureRoleAsync(roleManager, AppRoles.SuperAdmin.ToString());
            await EnsureRoleAsync(roleManager, AppRoles.Admin.ToString());
            await EnsureRoleAsync(roleManager, AppRoles.Basic.ToString());
        }

        private static async Task EnsureRoleAsync(
            RoleManager<AppRole> roleManager,
            string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new AppRole
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpperInvariant()
                });
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
