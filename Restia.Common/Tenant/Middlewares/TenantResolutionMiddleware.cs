using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Restia.Common.Tenant.Constants;
using Restia.Common.Tenant.Services.Interfaces;

namespace Restia.Common.Tenant.Middlewares
{
    public class TenantResolutionMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantResolutionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            ITenantService tenantService,
            ITenantContext tenantContext)
        {
            if (context.GetRouteValue(TenantConstants.RouteKey) is string routeTenant)
            {
                if (!await tenantService.IsValidActiveTenantAsync(routeTenant))
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    return;
                }

                tenantContext.SetTenant(routeTenant);
            }

            await _next(context);
        }
    }
}
