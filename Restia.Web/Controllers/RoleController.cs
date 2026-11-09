using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Restia.Common.Identity.Models;
using Restia.Common.Identity.Services.Interfaces;
using Restia.Common.Localization.Resources;
using Restia.Common.Permission.Constants;
using Restia.Common.Shared.Constants;
using Restia.Common.Shared.Models;
using Restia.Web.Models;
using X.PagedList;

namespace Restia.Web.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public RoleController(
            IRoleService roleService,
            IStringLocalizer<SharedResources> localizer)
        {
            _roleService = roleService;
            _localizer = localizer;
        }

        [Authorize(Policy = PermissionConstants.Role.Default)]
        public async Task<IActionResult> Index(
            string? searchTerm,
            string? orderBy,
            int page = 1,
            int pageSize = PaginationConstants.DefaultPageSize)
        {
            pageSize = PaginationConstants.NormalizePageSize(pageSize);
            page = Math.Max(1, page);

            var searchParam = new SearchParam
            {
                SearchTerm = searchTerm,
                OrderBy = orderBy
            };

            var result = await _roleService.GetPagedListAsync(searchParam, page, pageSize);

            var items = result.Items.Select(r => new RoleViewModel
            {
                Id = r.Id,
                Name = r.Name,
                PermissionCount = r.PermissionCount,
                UserCount = r.UserCount
            }).ToList();

            var model = new PaginationViewModel<RoleViewModel>
            {
                Items = new StaticPagedList<RoleViewModel>(
                    items, result.PageNumber, result.PageSize, result.TotalCount),
                SearchTerm = searchTerm,
                OrderBy = orderBy,
                PageSize = pageSize,
                SearchPlaceholderKey = "Role_SearchPlaceholder"
            };

            return View(model);
        }

        [Authorize(Policy = PermissionConstants.Role.Available)]
        public IActionResult Create()
        {
            return View(new RoleFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = PermissionConstants.Role.Available)]
        public async Task<IActionResult> Create(RoleFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _roleService.CreateAsync(new RoleCreateDto
            {
                Name = model.Name
            });

            if (!result.Succeeded)
            {
                if (!string.IsNullOrEmpty(result.ErrorCode))
                {
                    ModelState.AddModelError(string.Empty, _localizer[result.ErrorCode]);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View(model);
            }

            TempData["Success"] = "Role_CreateSuccess";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = PermissionConstants.Role.Available)]
        public async Task<IActionResult> Edit(string id)
        {
            var dto = await _roleService.GetInfoForUpdateAsync(id);
            if (dto == null)
            {
                return NotFound();
            }

            return View(new RoleFormViewModel
            {
                Id = dto.Id,
                Name = dto.Name
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = PermissionConstants.Role.Available)]
        public async Task<IActionResult> Edit(RoleFormViewModel model)
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _roleService.UpdateAsync(new RoleUpdateDto
            {
                Id = model.Id,
                Name = model.Name
            });

            if (result.ErrorCode == "Role_NotFound")
            {
                return NotFound();
            }

            if (!result.Succeeded)
            {
                if (!string.IsNullOrEmpty(result.ErrorCode))
                {
                    ModelState.AddModelError(string.Empty, _localizer[result.ErrorCode]);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View(model);
            }

            TempData["Success"] = "Role_EditSuccess";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = PermissionConstants.Role.Available)]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _roleService.DeleteAsync(id);

            if (result.ErrorCode == "Role_NotFound")
            {
                return NotFound();
            }

            if (result.ErrorCode == "Role_DeleteSuperAdmin")
            {
                TempData["Error"] = "Role_DeleteSuperAdmin";
                return RedirectToAction(nameof(Index));
            }

            if (result.ErrorCode == "Role_DeleteWhenInUse")
            {
                TempData["Error"] = "Role_DeleteWhenInUse";
                return RedirectToAction(nameof(Index));
            }

            TempData[result.Succeeded ? "Success" : "Error"] =
                result.Succeeded ? "Role_DeleteSuccess" : "Role_DeleteFailed";

            return RedirectToAction(nameof(Index));
        }
    }
}
