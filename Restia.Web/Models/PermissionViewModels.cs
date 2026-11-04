using Restia.Common.Permission.Constants;
using Restia.Common.Permission.Models;
using Restia.Common.Shared.Constants;
using X.PagedList;

namespace Restia.Web.Models
{
    public class PermissionItemInputModel
    {
        public string FormCode { get; set; } = string.Empty;
        public FormAuthoritySec Sec { get; set; } = FormAuthoritySec.None;
    }

    public class PermissionSaveViewModel
    {
        public PermissionTargetType Type { get; set; }
        public string TargetId { get; set; } = string.Empty;
        public string? SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = PaginationConstants.DefaultPageSize;
        public List<PermissionItemInputModel> Items { get; set; } = [];
    }

    public class PermissionMatrixRowViewModel
    {
        public string ParentFormNameKey { get; set; } = string.Empty;
        public string FormCode { get; set; } = string.Empty;
        public string FormNameKey { get; set; } = string.Empty;
        public FormAuthoritySec FormAuthoritySec { get; set; } = FormAuthoritySec.None;
    }

    public class PermissionPageViewModel
    {
        public PermissionTargetType Type { get; set; } = PermissionTargetType.Role;
        public string? TargetId { get; set; }
        public IReadOnlyList<PermissionTargetOptionDto> TargetOptions { get; set; } = [];
        public PermissionMatrixDto? Matrix { get; set; }
        public IPagedList<PermissionMatrixRowViewModel>? PagedRows { get; set; }
        public string? SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = PaginationConstants.DefaultPageSize;
        public bool CanModify { get; set; }
    }
}
