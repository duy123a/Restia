using Restia.Common.Localization.Resources;
using System.ComponentModel.DataAnnotations;

namespace Restia.Web.Models
{
    public class UserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = [];
        public bool IsLockedOut { get; set; }
    }

    public class UserFormViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(SharedResources), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(SharedResources), Name = "User_Email")]
        [EmailAddress(ErrorMessageResourceType = typeof(SharedResources), ErrorMessageResourceName = "User_EmailInvalid")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(SharedResources), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(SharedResources), Name = "User_DisplayName")]
        public string DisplayName { get; set; } = string.Empty;

        [Display(ResourceType = typeof(SharedResources), Name = "User_Password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(ResourceType = typeof(SharedResources), Name = "User_ConfirmPassword")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessageResourceType = typeof(SharedResources), ErrorMessageResourceName = "User_PasswordMismatch")]
        public string? ConfirmPassword { get; set; }

        public IList<string> SelectedRoles { get; set; } = [];
        public IList<RoleOptionViewModel> AvailableRoles { get; set; } = [];
    }
}
