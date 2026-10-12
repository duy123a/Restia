using Restia.Common.Identity.Enums;
using System.Security.Claims;

namespace Restia.Common.Identity.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static bool IsSuperAdmin(this ClaimsPrincipal principal)
        {
            return principal.IsInRole(AppRoles.SuperAdmin.ToString());
        }

        public static string GetLoginId(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        public static List<string> GetRoles(this ClaimsPrincipal principal)
        {
            return principal
                .FindAll(ClaimTypes.Role)
                .Select(c => c.Value)
                .Distinct()
                .ToList();
        }
    }
}
