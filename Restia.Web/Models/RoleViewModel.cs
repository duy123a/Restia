using Restia.Common.Localization.Resources;
using System.ComponentModel.DataAnnotations;

namespace Restia.Web.Models
{
    public class RoleViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int UserCount { get; set; }
        public int PermissionCount { get; set; }
    }

    public class RoleFormViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(SharedResources), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(SharedResources), Name = "Role_Name")]
        [StringLength(256)]
        public string Name { get; set; } = string.Empty;
    }
}
