using Restia.Common.Permission.Constants;

namespace Restia.Common.Identity.Models
{
    public record RoleGetDto(
        string Id,
        string Name,
        int PermissionCount,
        int UserCount);

    public record RolePermissionDto(
        string FormCode,
        FormAuthoritySec FormAuthoritySec
    );

    public class RoleCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public IReadOnlyList<RolePermissionDto> SelectedPermissions { get; set; } = [];
    }

    public class RoleUpdateDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public IReadOnlyList<RolePermissionDto> SelectedPermissions { get; set; } = [];
    }
}
