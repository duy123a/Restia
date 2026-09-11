using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restia.Common.Data.Extensions;
using Restia.Common.Identity.Extensions;

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

            return services;
        }
    }
}
