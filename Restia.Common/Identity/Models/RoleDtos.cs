namespace Restia.Common.Identity.Models
{
    public record RoleGetDto(
        string Id,
        string Name,
        int PermissionCount,
        int UserCount);

    public class RoleCreateDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public class RoleUpdateDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
