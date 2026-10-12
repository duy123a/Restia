using Restia.Common.Permission.Constants;
using Restia.Common.Shared.Entities;

namespace Restia.Common.Permission.Entities
{
    public class UserPermission : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;

        public string FormId { get; set; } = string.Empty;

        public FormAuthoritySec FormAuthoritySec { get; set; }
            = FormAuthoritySec.None;
    }
}
