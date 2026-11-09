using Restia.Common.Localization.Resources;
using System.ComponentModel.DataAnnotations;

namespace Restia.Web.Models
{
    public class TenantViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
    }

    public class TenantCreateViewModel
    {
        [Required(ErrorMessageResourceType = typeof(SharedResources), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(SharedResources), Name = "Tenant_Id")]
        [StringLength(63, MinimumLength = 2)]
        [RegularExpression("^[a-z][a-z0-9_]{1,62}$", ErrorMessageResourceType = typeof(SharedResources), ErrorMessageResourceName = "Tenant_InvalidId")]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(SharedResources), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(SharedResources), Name = "Tenant_DisplayName")]
        [StringLength(200)]
        public string DisplayName { get; set; } = string.Empty;

        [Display(ResourceType = typeof(SharedResources), Name = "Tenant_IsActive")]
        public bool IsActive { get; set; } = true;

        [Required(ErrorMessageResourceType = typeof(SharedResources), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(SharedResources), Name = "User_UserName")]
        public string AdminUserName { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(SharedResources), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(SharedResources), Name = "User_Email")]
        [EmailAddress(ErrorMessageResourceType = typeof(SharedResources), ErrorMessageResourceName = "User_EmailInvalid")]
        public string AdminEmail { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(SharedResources), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(SharedResources), Name = "User_Password")]
        [DataType(DataType.Password)]
        public string AdminPassword { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(SharedResources), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(SharedResources), Name = "User_ConfirmPassword")]
        [DataType(DataType.Password)]
        [Compare(nameof(AdminPassword), ErrorMessageResourceType = typeof(SharedResources), ErrorMessageResourceName = "User_PasswordMismatch")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class TenantEditViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(SharedResources), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(SharedResources), Name = "Tenant_DisplayName")]
        [StringLength(200)]
        public string DisplayName { get; set; } = string.Empty;

        [Display(ResourceType = typeof(SharedResources), Name = "Tenant_IsActive")]
        public bool IsActive { get; set; } = true;
    }
}
