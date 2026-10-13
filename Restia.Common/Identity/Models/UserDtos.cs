namespace Restia.Common.Identity.Models
{
    public record RoleOptionDto(string Id, string Name);

    public record UserGetDto(
        string Id,
        string Email,
        string DisplayName,
        IList<string> Roles,
        bool IsLockedOut);

    public class UserCreateDto
    {
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public IList<string> SelectedRoles { get; set; } = [];
    }

    public class UserUpdateDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Password { get; set; }
        public IList<string> SelectedRoles { get; set; } = [];
        public IReadOnlyList<RoleOptionDto> AvailableRoles { get; init; } = [];
    }
}
