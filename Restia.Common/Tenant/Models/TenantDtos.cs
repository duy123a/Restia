namespace Restia.Common.Tenant.Models
{
    public record TenantGetDto(
        string Id,
        string DisplayName,
        bool IsActive,
        DateTimeOffset? CreatedAt);

    public class TenantCreateDto
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string AdminUserName { get; set; } = string.Empty;
        public string AdminEmail { get; set; } = string.Empty;
        public string AdminPassword { get; set; } = string.Empty;
    }

    public class TenantUpdateDto
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
