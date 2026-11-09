using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restia.Common.Identity.Configurations;
using Restia.Common.Tenant.Constants;
using Restia.Common.Tenant.Services.Interfaces;
using IdentityConsts = Restia.Common.Identity.Constants.IdentityConstants;

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

                options.ReturnUrlParameter = IdentityConsts.DefaultReturnUrlParameter;

                options.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = async context =>
                    {
                        var tenantContext = context.HttpContext.RequestServices.GetRequiredService<ITenantContext>();
                        var claimSchema = context.Principal?.GetSchemaId();
                        if (!string.IsNullOrWhiteSpace(claimSchema) && TenantConstants.IsValidSchemaName(claimSchema))
                        {
                            tenantContext.SetTenant(claimSchema);
                        }

                        await SecurityStampValidator.ValidatePrincipalAsync(context);
                    },
                    OnRedirectToLogin = context =>
                    {
                        var tenantId = GetRouteTenantOrDefault(context.HttpContext);
                        var returnUrl = $"{context.Request.PathBase}{context.Request.Path}{context.Request.QueryString}";
                        var loginUrl = QueryHelpers.AddQueryString(
                            $"/{tenantId}{cookieSettings.LoginPath}",
                            IdentityConsts.DefaultReturnUrlParameter,
                            returnUrl);

                        context.Response.Redirect(loginUrl);
                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = context =>
                    {
                        var tenantId = GetRouteTenantOrDefault(context.HttpContext);
                        var returnUrl = $"{context.Request.PathBase}{context.Request.Path}{context.Request.QueryString}";
                        var deniedUrl = QueryHelpers.AddQueryString(
                            $"/{tenantId}{cookieSettings.AccessDeniedPath}",
                            IdentityConsts.DefaultReturnUrlParameter,
                            returnUrl);

                        context.Response.Redirect(deniedUrl);
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }

        private static string GetRouteTenantOrDefault(HttpContext httpContext)
        {
            var routeTenant = httpContext.GetRouteValue(TenantConstants.RouteKey)?.ToString();
            if (!string.IsNullOrWhiteSpace(routeTenant) && TenantConstants.IsValidSchemaName(routeTenant))
            {
                return routeTenant.ToLowerInvariant();
            }

            return TenantConstants.DefaultTenantId;
        }
    }
}
