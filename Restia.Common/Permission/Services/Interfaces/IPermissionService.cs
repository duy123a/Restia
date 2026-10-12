using Restia.Common.Permission.Constants;

namespace Restia.Common.Permission.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<FormAuthoritySec> GetAuthoritySecAsync(string userId, IEnumerable<string> roleIds, string formId);
    }
}
