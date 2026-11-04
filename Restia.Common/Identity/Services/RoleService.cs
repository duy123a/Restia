using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Restia.Common.Identity.Entities;
using Restia.Common.Identity.Enums;
using Restia.Common.Identity.Models;
using Restia.Common.Identity.Services.Interfaces;
using Restia.Common.Shared.Models;
using Restia.Common.Shared.UnitOfWorks.Interfaces;

namespace Restia.Common.Identity.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAdminUnitOfWork _uow;

        public RoleService(
            RoleManager<AppRole> roleManager,
            UserManager<AppUser> userManager,
            IAdminUnitOfWork uow)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _uow = uow;
        }

        public Task<PagedResult<RoleGetDto>> GetPagedListAsync(
            SearchParam? searchParam,
            int page,
            int pageSize)
        {
            return _uow.RoleRepository.GetPagedListAsync(searchParam, page, pageSize);
        }

        public async Task<RoleUpdateDto?> GetInfoForUpdateAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return null;

            return new RoleUpdateDto
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty
            };
        }

        public async Task<ServiceResult> CreateAsync(RoleCreateDto dto)
        {
            var strategy = _uow.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _uow.BeginTransactionAsync();

                try
                {
                    if (await _roleManager.RoleExistsAsync(dto.Name))
                        return ServiceResult.Fail("Role_DuplicateName");

                    var role = new AppRole
                    {
                        Name = dto.Name
                    };

                    var result = await _roleManager.CreateAsync(role);

                    if (!result.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        return ServiceResult.FromIdentityResult(result);
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

        public async Task<ServiceResult> UpdateAsync(RoleUpdateDto dto)
        {
            var strategy = _uow.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _uow.BeginTransactionAsync();

                try
                {
                    var role = await _roleManager.FindByIdAsync(dto.Id);

                    if (role == null)
                        return ServiceResult.Fail("Role_NotFound");

                    if (string.Equals(role.Name, dto.Name, StringComparison.Ordinal))
                        return ServiceResult.Success();

                    if (string.Equals(role.Name, AppRoles.SuperAdmin.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        return ServiceResult.Fail("Role_CannotRenameSuperAdmin");
                    }

                    if (await _roleManager.RoleExistsAsync(dto.Name))
                    {
                        return ServiceResult.Fail("Role_DuplicateName");
                    }

                    role.Name = dto.Name;

                    var result = await _roleManager.UpdateAsync(role);

                    if (!result.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        return ServiceResult.FromIdentityResult(result);
                    }

                    var invalidateResult = await InvalidateUsersInRoleAsync(role.Name!);

                    if (!invalidateResult.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        return ServiceResult.FromIdentityResult(invalidateResult);
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

        public async Task<ServiceResult> DeleteAsync(string id)
        {
            var strategy = _uow.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _uow.BeginTransactionAsync();

                try
                {
                    var role = await _roleManager.FindByIdAsync(id);

                    if (role == null)
                        return ServiceResult.Fail("Role_NotFound");

                    if (string.Equals(role.Name, AppRoles.SuperAdmin.ToString(), StringComparison.OrdinalIgnoreCase))
                        return ServiceResult.Fail("Role_DeleteSuperAdmin");

                    var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name ?? string.Empty);

                    if (usersInRole.Count > 0)
                        return ServiceResult.Fail("Role_DeleteWhenInUse");

                    // Delete orphan role permissions
                    var permissions = await _uow.RolePermissionRepository.FindAsync(rp => rp.RoleId == id);
                    foreach (var permission in permissions)
                    {
                        _uow.RolePermissionRepository.Delete(permission);
                    }
                    await _uow.SaveChangesAsync();

                    var result = await _roleManager.DeleteAsync(role);

                    if (!result.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        return ServiceResult.FromIdentityResult(result);
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

        // Force relogin to get a new role claim name
        private async Task<IdentityResult> InvalidateUsersInRoleAsync(string roleName)
        {
            var users = await _userManager.GetUsersInRoleAsync(roleName);

            foreach (var user in users)
            {
                var result = await _userManager.UpdateSecurityStampAsync(user);

                if (!result.Succeeded)
                    return result;
            }

            return IdentityResult.Success;
        }
    }
}
