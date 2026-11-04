using Restia.Common.Permission.Constants;
using Restia.Common.Shared.Entities;

namespace Restia.Common.Permission.Entities
{
    public class RolePermission : BaseAudit
    {
        public string RoleId { get; set; } = string.Empty;

        public string FormCode { get; set; } = string.Empty;

        public FormAuthoritySec FormAuthoritySec { get; set; }
            = FormAuthoritySec.None;
    }
}
