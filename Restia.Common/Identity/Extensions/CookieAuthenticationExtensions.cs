using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restia.Common.Identity.Configurations;
using Restia.Common.Identity.Constants;

namespace Restia.Common.Identity.Extensions
{
    public static class CookieAuthenticationExtensions
    {
        public static IServiceCollection UseCookieAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var cookieSettings = configuration.GetSection("CookieSettings").Get<CookieSettings>()
                     ?? throw new InvalidOperationException("CookieSettings section is missing.");

            services.Configure<CookieSettings>(configuration.GetSection("CookieSettings"));

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = cookieSettings.LoginPath;
                options.LogoutPath = cookieSettings.LogoutPath;
                options.AccessDeniedPath = cookieSettings.AccessDeniedPath;
                options.ExpireTimeSpan = TimeSpan.FromSeconds(cookieSettings.DefaultExpireSeconds);
                options.SlidingExpiration = cookieSettings.SlidingExpiration;
                options.Cookie.Path = cookieSettings.CookiePath;

                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

                options.ReturnUrlParameter = IdentityConstants.DefaultReturnUrlParameter;
            });

            return services;
        }
    }
}
