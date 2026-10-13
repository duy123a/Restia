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
using Restia.Common.Permission.Repository;
using Restia.Common.Permission.Repository.Interfaces;
using Restia.Common.Permission.Services;
using Restia.Common.Permission.Services.Interfaces;
using Restia.Common.Shared.Helpers;
using Restia.Common.Shared.UnitOfWorks;
using Restia.Common.Shared.UnitOfWorks.Interfaces;

namespace Restia.Common.Shared.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructureModules(this IServiceCollection services, IConfiguration configuration)
        {
            DateTimeOffsetHelper.Initialize(configuration);

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

            services.AddScoped<IFormRepository, FormRepository>();
            services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IAdminUnitOfWork, AdminUnitOfWork>();

            services.AddScoped<IFormService, FormService>();
            services.AddScoped<IPermissionService, PermissionService>();

            return services;
        }

        public static IServiceCollection AddHostServices(this IServiceCollection services)
        {
            services.AddHostedService<IdentitySeedWorker>();
            services.AddHostedService<FormSeedWorker>();

            return services;
        }
    }
}
