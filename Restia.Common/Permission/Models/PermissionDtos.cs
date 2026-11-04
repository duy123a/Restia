using Restia.Common.Permission.Constants;

namespace Restia.Common.Permission.Models
{
    public enum PermissionTargetType
    {
        Role = 1,
        User = 2
    }

    public record FormPermissionDto(
        string FormCode,
        FormAuthoritySec FormAuthoritySec);

    public record PermissionTargetOptionDto(
        string Id,
        string Name);

    public record PermissionMatrixItemDto(
        string FormCode,
        string FormNameKey,
        FormAuthoritySec FormAuthoritySec);

    public record PermissionMatrixGroupDto(
        string ParentFormNameKey,
        IReadOnlyList<PermissionMatrixItemDto> Items);

    public class PermissionMatrixDto
    {
        public PermissionTargetType TargetType { get; set; }
        public string TargetId { get; set; } = string.Empty;
        public string TargetName { get; set; } = string.Empty;
        public bool IsReadOnly { get; set; }
        public IReadOnlyList<PermissionMatrixGroupDto> Groups { get; set; } = [];
    }
}
