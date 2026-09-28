using Restia.Common.Localization.Resources;
using System.ComponentModel.DataAnnotations;

namespace Restia.Web.Models
{
    public class LoginViewModel
    {
        [Display(ResourceType = typeof(SharedResources), Name = "Login_Email")]
        [Required(
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(SharedResources))]
        public string Email { get; set; } = string.Empty;

        [Display(ResourceType = typeof(SharedResources), Name = "Login_Password")]
        [Required(
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(SharedResources))]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(ResourceType = typeof(SharedResources), Name = "Login_RememberMe")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
