using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restia.Common.Data.Extensions;
using Restia.Common.Form.Repository;
using Restia.Common.Form.Repository.Interfaces;
using Restia.Common.Form.Services;
using Restia.Common.Form.Services.Interfaces;
using Restia.Common.Identity.Extensions;
using Restia.Common.Identity.Repository;
using Restia.Common.Identity.Repository.Interfaces;
using Restia.Common.Identity.Services;
using Restia.Common.Identity.Services.Interfaces;
using Restia.Common.Notification.Repository;
using Restia.Common.Notification.Repository.Interfaces;
using Restia.Common.Notification.Service;
using Restia.Common.Notification.Service.Interfaces;
using Restia.Common.Permission.Repository;
using Restia.Common.Permission.Repository.Interfaces;
using Restia.Common.Permission.Services;
using Restia.Common.Permission.Services.Interfaces;
using Restia.Common.Shared.Helpers;
using Restia.Common.Shared.UnitOfWorks;
using Restia.Common.Shared.UnitOfWorks.Interfaces;
using Restia.Common.Tenant.Repository;
using Restia.Common.Tenant.Repository.Interfaces;
using Restia.Common.Tenant.Services;
using Restia.Common.Tenant.Services.Interfaces;

namespace Restia.Common.Shared.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructureModules(this IServiceCollection services, IConfiguration configuration)
        {
            DateTimeOffsetHelper.Initialize(configuration);

            services.AddMemoryCache();
            services.AddPersistenceModule();
            services.AddDataModule(configuration);
            services.AddIdentityModule(configuration);
            services.AddHostServices();
            services.AddAuthorization();

            return services;
        }

        public static IServiceCollection AddPersistenceModule(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddHttpClient();
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();

            services.AddScoped<ITenantContext, TenantContext>();

            services.AddScoped<IFormRepository, FormRepository>();
            services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<ITenantRepository, TenantRepository>();
            services.AddScoped<IAdminUnitOfWork, AdminUnitOfWork>();

            services.AddScoped<IFormService, FormService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAppNotificationService, AppNotificationService>();
            services.AddScoped<ITenantService, TenantService>();

            return services;
        }

        public static IServiceCollection AddHostServices(this IServiceCollection services)
        {
            services.AddHostedService<IdentitySeedWorker>();

            return services;
        }
    }
}
