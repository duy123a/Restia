using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restia.Common.Data.Extensions;
using Restia.Common.Form.Repository;
using Restia.Common.Form.Repository.Interfaces;
using Restia.Common.Form.Services;
using Restia.Common.Form.Services.Interfaces;
using Restia.Common.Identity.Extensions;
using Restia.Common.Permission.Repository;
using Restia.Common.Permission.Repository.Interfaces;
using Restia.Common.Permission.Services;
using Restia.Common.Permission.Services.Interfaces;

namespace Restia.Common.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructureModules(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddPersistenceModule(configuration);
            services.AddDataModule(configuration);
            services.AddIdentityModule(configuration);
            services.AddAuthorization();

            return services;
        }

        public static IServiceCollection AddPersistenceModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            services.AddScoped<IFormRepository, FormRepository>();
            services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();

            services.AddScoped<IFormService, FormService>();
            services.AddScoped<IPermissionService, PermissionService>();

            return services;
        }
    }
}
