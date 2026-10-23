using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Restia.Common.Identity.Models;
using Restia.Common.Identity.Services.Interfaces;
using Restia.Common.Localization.Resources;
using Restia.Common.Permission.Constants;
using Restia.Common.Shared.Constants;
using Restia.Web.Models;
using X.PagedList;

namespace Restia.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public UserController(
            IUserService userService,
            IStringLocalizer<SharedResources> localizer)
        {
            _userService = userService;
            _localizer = localizer;
        }

        [Authorize(Policy = PermissionConstants.User.Default)]
        public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = PaginationConstants.DefaultPageSize)
        {
            pageSize = PaginationConstants.NormalizePageSize(pageSize);

            var result = await _userService.GetPagedListAsync(search, page, pageSize);

            var items = result.Items.Select(u => new UserViewModel
            {
                Id = u.Id,
                Email = u.Email,
                DisplayName = u.DisplayName,
                Roles = u.Roles,
                IsLockedOut = u.IsLockedOut
            }).ToList();

            var model = new PaginationViewModel<UserViewModel>
            {
                Items = new StaticPagedList<UserViewModel>(
                    items, result.PageNumber, result.PageSize, result.TotalCount),
                Search = search,
                PageSize = pageSize,
                SearchPlaceholderKey = "User_Placeholder"
            };

            return View(model);
        }

        [Authorize(Policy = PermissionConstants.User.Available)]
        public async Task<IActionResult> Create()
        {
            var model = new UserFormViewModel
            {
                AvailableRoles = await BuildRoleOptionsAsync([])
            };

            return View(model);
        }

        [Authorize(Policy = PermissionConstants.User.Available)]
        public async Task<IActionResult> Edit(string id)
        {
            var dto = await _userService.GetInfoForUpdateAsync(id);
            if (dto == null) return NotFound();

            var selectedSet = dto.SelectedRoles.ToHashSet(StringComparer.OrdinalIgnoreCase);

            return View(new UserFormViewModel
            {
                Id = dto.Id,
                Email = dto.Email,
                DisplayName = dto.DisplayName,
                SelectedRoles = dto.SelectedRoles,
                AvailableRoles = dto.AvailableRoles.Select(r => new RoleOptionViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    IsSelected = selectedSet.Contains(r.Name)
                }).ToList()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = PermissionConstants.User.Available)]
        public async Task<IActionResult> Edit(UserFormViewModel model)
        {
            if (string.IsNullOrEmpty(model.Id)) return NotFound();

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.Remove(nameof(model.Password));
                ModelState.Remove(nameof(model.ConfirmPassword));
            }

            if (!ModelState.IsValid)
            {
                model.AvailableRoles = await BuildRoleOptionsAsync(model.SelectedRoles);
                return View(model);
            }

            var result = await _userService.UpdateAsync(new UserUpdateDto
            {
                Id = model.Id,
                Email = model.Email,
                DisplayName = model.DisplayName,
                Password = model.Password,
                SelectedRoles = model.SelectedRoles
            });

            if (result.ErrorCode == "User_NotFound") return NotFound();

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error);

                model.AvailableRoles = await BuildRoleOptionsAsync(model.SelectedRoles);
                return View(model);
            }

            TempData["Success"] = "User_SaveSuccess";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = PermissionConstants.User.Available)]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _userService.DeleteAsync(id, User.Identity?.Name ?? string.Empty);

            if (result.ErrorCode == "User_NotFound") return NotFound();

            if (result.ErrorCode == "User_CannotDeleteYourself")
            {
                TempData["Error"] = "User_CannotDeleteYourself";
                return RedirectToAction(nameof(Index));
            }

            TempData[result.Succeeded ? "Success" : "Error"] =
                result.Succeeded ? "User_DeleteSuccess" : "User_DeleteFailed";

            return RedirectToAction(nameof(Index));
        }

        private async Task<IList<RoleOptionViewModel>> BuildRoleOptionsAsync(IEnumerable<string> selected)
        {
            var selectedSet = selected.ToHashSet(StringComparer.OrdinalIgnoreCase);
            var roles = await _userService.GetAllRoleOptionsAsync();

            return roles.Select(r => new RoleOptionViewModel
            {
                Id = r.Id,
                Name = r.Name,
                IsSelected = selectedSet.Contains(r.Name)
            }).ToList();
        }
    }
}
