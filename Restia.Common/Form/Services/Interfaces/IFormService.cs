using Restia.Common.Form.Models;
using System.Security.Claims;

namespace Restia.Common.Form.Services.Interfaces
{
    public interface IFormService
    {
        Task<IReadOnlyList<FormItemDto>> GetSidebarMenuAsync(ClaimsPrincipal user);
    }
}
