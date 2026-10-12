using Restia.Common.Permission.Constants;

namespace Restia.Common.Permission.Entities
{
    public class RolePermission
    {
        public string RoleId { get; set; } = string.Empty;

        public string FormId { get; set; } = string.Empty;

        public FormAuthoritySec FormAuthoritySec { get; set; }
            = FormAuthoritySec.None;
    }
}
