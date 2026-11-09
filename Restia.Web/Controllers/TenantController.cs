using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Restia.Common.Localization.Resources;
using Restia.Common.Permission.Constants;
using Restia.Common.Shared.Constants;
using Restia.Common.Shared.Models;
using Restia.Common.Tenant.Models;
using Restia.Common.Tenant.Services.Interfaces;
using Restia.Web.Models;
using X.PagedList;

namespace Restia.Web.Controllers
{
    [Authorize(Policy = PermissionConstants.Tenant.Available)]
    public class TenantController : Controller
    {
        private readonly ITenantService _tenantService;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public TenantController(
            ITenantService tenantService,
            IStringLocalizer<SharedResources> localizer)
        {
            _tenantService = tenantService;
            _localizer = localizer;
        }

        public async Task<IActionResult> Index(
            string? searchTerm,
            string? orderBy,
            int page = 1,
            int pageSize = PaginationConstants.DefaultPageSize)
        {
            pageSize = PaginationConstants.NormalizePageSize(pageSize);
            page = Math.Max(1, page);

            var result = await _tenantService.GetPagedListAsync(
                new SearchParam { SearchTerm = searchTerm, OrderBy = orderBy },
                page,
                pageSize);

            var items = result.Items.Select(t => new TenantViewModel
            {
                Id = t.Id,
                DisplayName = t.DisplayName,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt
            }).ToList();

            var model = new PaginationViewModel<TenantViewModel>
            {
                Items = new StaticPagedList<TenantViewModel>(
                    items, result.PageNumber, result.PageSize, result.TotalCount),
                SearchTerm = searchTerm,
                OrderBy = orderBy,
                PageSize = pageSize,
                SearchPlaceholderKey = "Tenant_SearchPlaceholder"
            };

            return View(model);
        }

        public IActionResult Create()
        {
            return View(new TenantCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TenantCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _tenantService.CreateAsync(new TenantCreateDto
            {
                Id = model.Id,
                DisplayName = model.DisplayName,
                IsActive = model.IsActive,
                AdminUserName = model.AdminUserName,
                AdminEmail = model.AdminEmail,
                AdminPassword = model.AdminPassword
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

            TempData["Success"] = "Tenant_CreateSuccess";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var dto = await _tenantService.GetInfoForUpdateAsync(id);
            if (dto == null)
            {
                return NotFound();
            }

            return View(new TenantEditViewModel
            {
                Id = dto.Id,
                DisplayName = dto.DisplayName,
                IsActive = dto.IsActive
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TenantEditViewModel model)
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _tenantService.UpdateAsync(new TenantUpdateDto
            {
                Id = model.Id,
                DisplayName = model.DisplayName,
                IsActive = model.IsActive
            });

            if (result.ErrorCode == "Tenant_NotFound")
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

            TempData["Success"] = "Tenant_EditSuccess";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _tenantService.DeleteAsync(id);

            if (result.ErrorCode == "Tenant_NotFound")
            {
                return NotFound();
            }

            if (!result.Succeeded)
            {
                TempData["Error"] = !string.IsNullOrEmpty(result.ErrorCode)
                    ? result.ErrorCode
                    : "Tenant_DeleteFailed";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Tenant_DeleteSuccess";
            return RedirectToAction(nameof(Index));
        }
    }
}
