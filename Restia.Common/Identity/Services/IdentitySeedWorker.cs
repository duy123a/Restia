using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Restia.Common.Entities;
using Restia.Common.Identity.Configurations;
using Restia.Common.Identity.Enums;

namespace Restia.Common.Identity.Services
{
    public sealed class IdentitySeedWorker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly SeedAdminSettings _seedOptions;

        public IdentitySeedWorker(
            IServiceProvider serviceProvider,
            IOptions<SeedAdminSettings> seedOptions)
        {
            _serviceProvider = serviceProvider;
            _seedOptions = seedOptions.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            await EnsureRoleAsync(roleManager, AppRoles.SuperAdmin.ToString());
            await EnsureRoleAsync(roleManager, AppRoles.Admin.ToString());
            await EnsureRoleAsync(roleManager, AppRoles.Basic.ToString());

            await EnsureAdminUserAsync(userManager);
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

        private async Task EnsureAdminUserAsync(
            UserManager<AppUser> userManager)
        {
            var admin = await userManager.FindByEmailAsync(_seedOptions.AdminEmail);
            if (admin != null)
            {
                return;
            }

            admin = new AppUser
            {
                Email = _seedOptions.AdminEmail,
                UserName = _seedOptions.AdminEmail,
                EmailConfirmed = true,
                DisplayName = AppRoles.SuperAdmin.ToString()
            };

            var result = await userManager.CreateAsync(admin, _seedOptions.AdminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, AppRoles.SuperAdmin.ToString());
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
