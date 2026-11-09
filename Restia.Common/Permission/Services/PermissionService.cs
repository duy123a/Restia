using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Restia.Common.Identity.Entities;
using Restia.Common.Identity.Enums;
using Restia.Common.Identity.Extensions;
using Restia.Common.Permission.Constants;
using Restia.Common.Permission.Entities;
using Restia.Common.Permission.Models;
using Restia.Common.Permission.Requirements;
using Restia.Common.Permission.Services.Interfaces;
using Restia.Common.Shared.Models;
using Restia.Common.Shared.UnitOfWorks.Interfaces;
using Restia.Common.Tenant.Services.Interfaces;
using System.Security.Claims;

namespace Restia.Common.Permission.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IAdminUnitOfWork _uow;
        private readonly ITenantContext _tenantContext;

        public PermissionService(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IAdminUnitOfWork uow,
            ITenantContext tenantContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _uow = uow;
            _tenantContext = tenantContext;
        }

        public async Task<FormAuthoritySec> GetAuthoritySecAsync(
            string userId,
            IEnumerable<string> roleNames,
            string formCode)
        {
            var userAuthority = await _uow.UserPermissionRepository.GetByIdAsync(userId, formCode);
            if (userAuthority != null && userAuthority.FormAuthoritySec != FormAuthoritySec.None)
            {
                return userAuthority.FormAuthoritySec;
            }

            var roleNamesList = roleNames
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (roleNamesList.Count > 0)
            {
                var roleAuthorities = await _uow.RolePermissionRepository.GetRoleAuthorityAsync(roleNamesList, formCode);

                if (roleAuthorities.Contains(FormAuthoritySec.Unavailable))
                {
                    return FormAuthoritySec.Unavailable;
                }

                if (roleAuthorities.Contains(FormAuthoritySec.Available))
                {
                    return FormAuthoritySec.Available;
                }

                if (roleAuthorities.Contains(FormAuthoritySec.ReferablePrintable))
                {
                    return FormAuthoritySec.ReferablePrintable;
                }
            }

            return FormAuthoritySec.Unavailable;
        }

        public async Task<bool> HasPermissionAsync(
            ClaimsPrincipal user,
            string policyName)
        {
            if (user.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            var requirement = new PermissionRequirement(policyName);

            if (string.Equals(requirement.Screen, PermissionConstants.FormCodes.Tenant, StringComparison.OrdinalIgnoreCase))
            {
                return _tenantContext.IsPublic && user.IsSuperAdmin();
            }

            if (user.IsSuperAdmin())
            {
                return true;
            }

            var authoritySec = await GetAuthoritySecAsync(
                user.GetUserId(),
                user.GetRoles(),
                requirement.Screen);

            if (requirement.FormAuthoritySec == null)
            {
                return authoritySec != FormAuthoritySec.None
                    && authoritySec != FormAuthoritySec.Unavailable;
            }

            return authoritySec == requirement.FormAuthoritySec;
        }

        public async Task<IReadOnlyList<PermissionTargetOptionDto>> GetTargetOptionsAsync(PermissionTargetType targetType)
        {
            if (targetType == PermissionTargetType.Role)
            {
                return await _uow.RolePermissionRepository.GetRolePermissionTargetOptionsAsync();
            }

            return await _uow.UserPermissionRepository.GetUserPermissionTargetOptionsAsync();
        }

        public async Task<PermissionMatrixDto?> GetMatrixAsync(
            PermissionTargetType targetType,
            string targetId)
        {
            if (string.IsNullOrWhiteSpace(targetId))
                return null;

            string targetName;
            bool isReadOnly = false;
            Dictionary<string, FormAuthoritySec> existingPerms;

            if (targetType == PermissionTargetType.Role)
            {
                var role = await _roleManager.FindByIdAsync(targetId);
                if (role == null) return null;

                targetName = role.Name ?? string.Empty;
                isReadOnly = string.Equals(
                    role.Name,
                    AppRoles.SuperAdmin.ToString(),
                    StringComparison.OrdinalIgnoreCase);

                var rolePermissions = await _uow.RolePermissionRepository
                    .FindAsync(rp => rp.RoleId == targetId);

                existingPerms = rolePermissions
                    .ToDictionary(
                        rp => rp.FormCode,
                        rp => rp.FormAuthoritySec,
                        StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                var user = await _userManager.FindByIdAsync(targetId);
                if (user == null) return null;

                targetName = user.UserName ?? user.Email ?? string.Empty;
                isReadOnly = await _userManager.IsInRoleAsync(user, AppRoles.SuperAdmin.ToString());

                var userPermissions = await _uow.UserPermissionRepository
                    .FindAsync(up => up.UserId == targetId);

                existingPerms = userPermissions
                    .ToDictionary(
                        up => up.FormCode,
                        up => up.FormAuthoritySec,
                        StringComparer.OrdinalIgnoreCase);
            }

            var allForms = await _uow.FormRepository
                .FindAsync(f => f.IsActive && f.Code != PermissionConstants.FormCodes.Tenant);

            var rootForms = allForms
                .Where(f => f.ParentId == null)
                .OrderBy(f => f.DisplayOrder)
                .ToList();

            var activeChildren = allForms
                .Where(f => f.ParentId.HasValue)
                .GroupBy(f => f.ParentId!.Value)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(f => f.DisplayOrder).ToList());

            var groups = new List<PermissionMatrixGroupDto>();

            foreach (var root in rootForms)
            {
                var children = activeChildren.TryGetValue(root.Id, out var items)
                    ? items
                    : [];

                var leafChildren = children
                    .Where(c => !string.IsNullOrWhiteSpace(c.Url))
                    .Select(c => new PermissionMatrixItemDto(
                        c.Code,
                        c.Name,
                        isReadOnly
                            ? FormAuthoritySec.Available
                            : (existingPerms.TryGetValue(c.Code, out var sec)
                                ? sec
                                : FormAuthoritySec.None)))
                    .ToList();

                if (leafChildren.Count > 0)
                {
                    groups.Add(new PermissionMatrixGroupDto(root.Name, leafChildren));
                }
                else if (!string.IsNullOrWhiteSpace(root.Url))
                {
                    var selfItem = new PermissionMatrixItemDto(
                        root.Code,
                        root.Name,
                        isReadOnly
                            ? FormAuthoritySec.Available
                            : (existingPerms.TryGetValue(root.Code, out var sec)
                                ? sec
                                : FormAuthoritySec.None));

                    groups.Add(new PermissionMatrixGroupDto(root.Name, [selfItem]));
                }
            }

            return new PermissionMatrixDto
            {
                TargetType = targetType,
                TargetId = targetId,
                TargetName = targetName,
                IsReadOnly = isReadOnly,
                Groups = groups
            };
        }

        public async Task<ServiceResult> SaveMatrixAsync(
            PermissionTargetType targetType,
            string targetId,
            IReadOnlyList<FormPermissionDto> items)
        {
            if (string.IsNullOrWhiteSpace(targetId))
            {
                return ServiceResult.Fail("Target_NotFound");
            }

            var strategy = _uow.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _uow.BeginTransactionAsync();

                try
                {
                    // Validate target
                    AppRole? role = null;
                    AppUser? user = null;

                    if (targetType == PermissionTargetType.Role)
                    {
                        role = await _roleManager.FindByIdAsync(targetId);

                        if (role == null)
                            return ServiceResult.Fail("Role_NotFound");

                        if (string.Equals(
                            role.Name,
                            AppRoles.SuperAdmin.ToString(),
                            StringComparison.OrdinalIgnoreCase))
                        {
                            return ServiceResult.Fail(
                                "Role_CannotModifySuperAdminPermissions");
                        }
                    }
                    else
                    {
                        user = await _userManager.FindByIdAsync(targetId);

                        if (user == null)
                            return ServiceResult.Fail("User_NotFound");

                        if (await _userManager.IsInRoleAsync(user, AppRoles.SuperAdmin.ToString()))
                        {
                            return ServiceResult.Fail(
                                "Role_CannotModifySuperAdminPermissions");
                        }
                    }

                    // Get active leaf forms
                    var activeLeafForms = await _uow.FormRepository
                        .FindAsync(f =>
                            f.IsActive &&
                            f.Code != PermissionConstants.FormCodes.Tenant &&
                            f.Url != null &&
                            f.Url != "");

                    var activeLeafFormCodes = activeLeafForms
                        .Select(f => f.Code)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    var activeCodeSet = activeLeafFormCodes
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    // Normalize submitted permissions (only active leaf forms)
                    var normalizedItems = items
                        .Where(i => activeCodeSet.Contains(i.FormCode))
                        .GroupBy(i => i.FormCode, StringComparer.OrdinalIgnoreCase)
                        .Select(g => g.Last())
                        .Select(i => new FormPermissionDto(
                            i.FormCode,
                            i.FormAuthoritySec switch
                            {
                                FormAuthoritySec.Available => FormAuthoritySec.Available,
                                FormAuthoritySec.ReferablePrintable => FormAuthoritySec.ReferablePrintable,
                                FormAuthoritySec.Unavailable => FormAuthoritySec.Unavailable,
                                _ => FormAuthoritySec.None
                            }))
                        .ToList();

                    var submittedCodes = normalizedItems
                        .Select(i => i.FormCode)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    if (targetType == PermissionTargetType.Role)
                    {
                        var existing = await _uow.RolePermissionRepository
                            .FindAsync(rp => rp.RoleId == targetId && submittedCodes.Contains(rp.FormCode));
                        var existingDict = existing.ToDictionary(rp => rp.FormCode, StringComparer.OrdinalIgnoreCase);

                        foreach (var item in normalizedItems)
                        {
                            if (item.FormAuthoritySec == FormAuthoritySec.None)
                            {
                                if (existingDict.TryGetValue(item.FormCode, out var toDelete))
                                {
                                    _uow.RolePermissionRepository.Delete(toDelete);
                                }
                            }
                            else
                            {
                                if (existingDict.TryGetValue(item.FormCode, out var toUpdate))
                                {
                                    toUpdate.FormAuthoritySec = item.FormAuthoritySec;
                                    _uow.RolePermissionRepository.Update(toUpdate);
                                }
                                else
                                {
                                    await _uow.RolePermissionRepository.AddAsync(new RolePermission
                                    {
                                        RoleId = targetId,
                                        FormCode = item.FormCode,
                                        FormAuthoritySec = item.FormAuthoritySec
                                    });
                                }
                            }
                        }

                        await _uow.SaveChangesAsync();

                        // Refresh security stamps for users in this role
                        var usersInRole = await _userManager.GetUsersInRoleAsync(role!.Name!);

                        foreach (var u in usersInRole)
                        {
                            await _userManager.UpdateSecurityStampAsync(u);
                        }
                    }
                    else
                    {
                        var existing = await _uow.UserPermissionRepository
                            .FindAsync(up => up.UserId == targetId && submittedCodes.Contains(up.FormCode));
                        var existingDict = existing.ToDictionary(up => up.FormCode, StringComparer.OrdinalIgnoreCase);

                        foreach (var item in normalizedItems)
                        {
                            if (item.FormAuthoritySec == FormAuthoritySec.None)
                            {
                                if (existingDict.TryGetValue(item.FormCode, out var toDelete))
                                {
                                    _uow.UserPermissionRepository.Delete(toDelete);
                                }
                            }
                            else
                            {
                                if (existingDict.TryGetValue(item.FormCode, out var toUpdate))
                                {
                                    toUpdate.FormAuthoritySec = item.FormAuthoritySec;
                                    _uow.UserPermissionRepository.Update(toUpdate);
                                }
                                else
                                {
                                    await _uow.UserPermissionRepository.AddAsync(new UserPermission
                                    {
                                        UserId = targetId,
                                        FormCode = item.FormCode,
                                        FormAuthoritySec = item.FormAuthoritySec
                                    });
                                }
                            }
                        }

                        await _uow.SaveChangesAsync();

                        await _userManager.UpdateSecurityStampAsync(user!);
                    }

                    await transaction.CommitAsync();

                    return ServiceResult.Success();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }
    }
}
