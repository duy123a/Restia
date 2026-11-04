using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using Restia.Common.Localization.Resources;
using Restia.Common.Permission.Constants;
using Restia.Common.Permission.Models;
using Restia.Common.Permission.Services.Interfaces;
using Restia.Common.Shared.Constants;
using Restia.Web.Models;
using X.PagedList;

namespace Restia.Web.Controllers
{
    public class PermissionController : Controller
    {
        private readonly IPermissionService _permissionService;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public PermissionController(
            IPermissionService permissionService,
            IStringLocalizer<SharedResources> localizer)
        {
            _permissionService = permissionService;
            _localizer = localizer;
        }

        [Authorize(Policy = PermissionConstants.Permission.Default)]
        public async Task<IActionResult> Index(
            PermissionTargetType type = PermissionTargetType.Role,
            string? id = null,
            string? searchTerm = null,
            int page = 1,
            int pageSize = PaginationConstants.DefaultPageSize)
        {
            if (!Enum.IsDefined(typeof(PermissionTargetType), type))
            {
                return NotFound();
            }

            pageSize = PaginationConstants.NormalizePageSize(pageSize);
            page = Math.Max(1, page);

            var canModify = await _permissionService.HasPermissionAsync(User, PermissionConstants.Permission.Available);
            var options = await _permissionService.GetTargetOptionsAsync(type);

            PermissionMatrixDto? matrix = null;
            IPagedList<PermissionMatrixRowViewModel>? pagedRows = null;

            if (!string.IsNullOrWhiteSpace(id))
            {
                matrix = await _permissionService.GetMatrixAsync(type, id);
                if (matrix == null)
                {
                    return NotFound();
                }

                var allLeafItems = matrix.Groups
                    .SelectMany(g => g.Items.Select(i => new PermissionMatrixRowViewModel
                    {
                        ParentFormNameKey = g.ParentFormNameKey,
                        FormCode = i.FormCode,
                        FormNameKey = i.FormNameKey,
                        FormAuthoritySec = i.FormAuthoritySec
                    }))
                    .ToList();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var term = searchTerm.Trim();
                    allLeafItems = allLeafItems.Where(i =>
                        i.FormCode.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                        i.FormNameKey.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                        _localizer[i.FormNameKey].Value.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                        _localizer[i.ParentFormNameKey].Value.Contains(term, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                var totalCount = allLeafItems.Count;
                var pagedItems = allLeafItems
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                pagedRows = new StaticPagedList<PermissionMatrixRowViewModel>(
                    pagedItems, page, pageSize, totalCount);
            }

            var model = new PermissionPageViewModel
            {
                Type = type,
                TargetId = id,
                TargetOptions = options,
                Matrix = matrix,
                PagedRows = pagedRows,
                SearchTerm = searchTerm,
                Page = page,
                PageSize = pageSize,
                CanModify = canModify
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = PermissionConstants.Permission.Available)]
        public async Task<IActionResult> Save(PermissionSaveViewModel model)
        {
            if (!Enum.IsDefined(typeof(PermissionTargetType), model.Type))
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(model.TargetId))
            {
                return BadRequest();
            }

            var items = model.Items
                .Select(i => new FormPermissionDto(i.FormCode, i.Sec))
                .ToList();

            var result = await _permissionService.SaveMatrixAsync(model.Type, model.TargetId, items);

            if (!result.Succeeded)
            {
                TempData["Error"] = !string.IsNullOrEmpty(result.ErrorCode)
                    ? result.ErrorCode
                    : "Permission_SaveFailed";
            }
            else
            {
                TempData["Success"] = "Permission_SaveSuccess";
            }

            var url = QueryHelpers.AddQueryString(
                Url.Action(nameof(Index))!,
                new Dictionary<string, string?>
                {
                    ["type"] = model.Type.ToString(),
                    ["id"] = model.TargetId,
                    ["searchTerm"] = model.SearchTerm,
                    ["page"] = model.Page.ToString(),
                    ["pageSize"] = model.PageSize.ToString()
                });

            return Redirect(url);
        }
    }
}
