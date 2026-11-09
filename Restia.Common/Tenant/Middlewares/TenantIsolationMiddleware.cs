using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Restia.Common.Identity.Extensions;
using Restia.Common.Tenant.Constants;
using Restia.Common.Tenant.Services.Interfaces;

namespace Restia.Common.Tenant.Middlewares
{
    public class TenantIsolationMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantIsolationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            ITenantContext tenantContext)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userSchema = context.User.GetSchemaId();
                if (context.GetRouteValue(TenantConstants.RouteKey) is string routeTenant)
                {
                    if (!string.Equals(userSchema, routeTenant, StringComparison.OrdinalIgnoreCase))
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        return;
                    }
                }
                else if (TenantConstants.IsValidSchemaName(userSchema))
                {
                    tenantContext.SetTenant(userSchema);
                }
            }

            await _next(context);
        }
    }
}
