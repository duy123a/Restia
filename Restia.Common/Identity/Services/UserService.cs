using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Restia.Common.Identity.Entities;
using Restia.Common.Identity.Enums;
using Restia.Common.Identity.Models;
using Restia.Common.Identity.Services.Interfaces;
using Restia.Common.Shared.Models;
using Restia.Common.Shared.UnitOfWorks.Interfaces;
using Restia.Common.Tenant.Services.Interfaces;

namespace Restia.Common.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IAdminUnitOfWork _uow;
        private readonly ITenantContext _tenantContext;

        public UserService(
            UserManager<AppUser> userManager,
            IAdminUnitOfWork uow,
            ITenantContext tenantContext)
        {
            _userManager = userManager;
            _uow = uow;
            _tenantContext = tenantContext;
        }

        public Task<PagedResult<UserGetDto>> GetPagedListAsync(SearchParam? searchParam, int page, int pageSize)
        {
            return _uow.UserRepository.GetPagedListAsync(searchParam, page, pageSize);
        }

        public async Task<UserUpdateDto?> GetInfoForUpdateAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;

            var selectedRoles = await _userManager.GetRolesAsync(user);
            var availableRoles = await _uow.RoleRepository.GetAllRoleOptionsAsync();
            var isLockedOut = await _userManager.IsLockedOutAsync(user);

            return new UserUpdateDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                DisplayName = user.DisplayName,
                PhoneNumber = user.PhoneNumber,
                SelectedRoles = selectedRoles.ToList(),
                AvailableRoles = availableRoles,
                IsLockedOut = isLockedOut,
                LockoutEnd = isLockedOut ? user.LockoutEnd : null
            };
        }

        public Task<IReadOnlyList<RoleOptionDto>> GetAllRoleOptionsAsync()
            => _uow.RoleRepository.GetAllRoleOptionsAsync();

        public async Task<ServiceResult> CreateAsync(UserCreateDto dto)
        {
            if (dto.SelectedRoles.Any(r => string.Equals(r, AppRoles.SuperAdmin.ToString(), StringComparison.OrdinalIgnoreCase)))
            {
                return ServiceResult.Fail("User_CannotAssignSuperAdminRole");
            }

            var strategy = _uow.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(
                state: dto,
                operation: async (_, state, cancellationToken) =>
                {
                    await using var transaction = await _uow.BeginTransactionAsync();

                    try
                    {
                        var user = new AppUser
                        {
                            UserName = dto.UserName,
                            Email = dto.Email,
                            PhoneNumber = dto.PhoneNumber,
                            DisplayName = dto.DisplayName,
                            SchemaId = _tenantContext.SchemaId,
                            EmailConfirmed = true
                        };

                        var result = await _userManager.CreateAsync(user, dto.Password);

                        if (!result.Succeeded)
                        {
                            await transaction.RollbackAsync();
                            return ServiceResult.FromIdentityResult(result);
                        }

                        if (dto.SelectedRoles.Count > 0)
                        {
                            var roleResult = await _userManager.AddToRolesAsync(
                                user, dto.SelectedRoles);

                            if (!roleResult.Succeeded)
                            {
                                await transaction.RollbackAsync();
                                return ServiceResult.FromIdentityResult(roleResult);
                            }
                        }

                        await transaction.CommitAsync();

                        return ServiceResult.Success();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                },
                verifySucceeded: null,
                cancellationToken: CancellationToken.None);
        }

        public async Task<ServiceResult> UpdateAsync(UserUpdateDto dto)
        {
            var strategy = _uow.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(
                state: dto,
                operation: async (_, state, cancellationToken) =>
                {
                    await using var transaction = await _uow.BeginTransactionAsync();

                    try
                    {
                        var user = await _userManager.FindByIdAsync(dto.Id);

                        if (user == null)
                            return ServiceResult.Fail("User_NotFound");

                        var currentRoles = await _userManager.GetRolesAsync(user);

                        var rolesToRemove = currentRoles
                            .Except(dto.SelectedRoles, StringComparer.OrdinalIgnoreCase)
                            .ToArray();

                        var rolesToAdd = dto.SelectedRoles
                            .Except(currentRoles, StringComparer.OrdinalIgnoreCase)
                            .ToArray();

                        if (rolesToRemove.Any(r => string.Equals(r, AppRoles.SuperAdmin.ToString(), StringComparison.OrdinalIgnoreCase)))
                        {
                            return ServiceResult.Fail("User_CannotRemoveSuperAdminRole");
                        }

                        if (rolesToAdd.Any(r => string.Equals(r, AppRoles.SuperAdmin.ToString(), StringComparison.OrdinalIgnoreCase)))
                        {
                            return ServiceResult.Fail("User_CannotAssignSuperAdminRole");
                        }

                        user.UserName = dto.UserName;
                        user.Email = dto.Email;
                        user.PhoneNumber = dto.PhoneNumber;
                        user.DisplayName = dto.DisplayName;

                        var updateResult = await _userManager.UpdateAsync(user);

                        if (!updateResult.Succeeded)
                        {
                            await transaction.RollbackAsync();
                            return ServiceResult.FromIdentityResult(updateResult);
                        }

                        if (!string.IsNullOrWhiteSpace(dto.Password))
                        {
                            var token =
                                await _userManager.GeneratePasswordResetTokenAsync(user);

                            var passwordResult =
                                await _userManager.ResetPasswordAsync(
                                    user, token, dto.Password);

                            if (!passwordResult.Succeeded)
                            {
                                await transaction.RollbackAsync();
                                return ServiceResult.FromIdentityResult(passwordResult);
                            }
                        }

                        if (rolesToRemove.Length > 0)
                        {
                            var removeResult =
                                await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

                            if (!removeResult.Succeeded)
                            {
                                await transaction.RollbackAsync();
                                return ServiceResult.FromIdentityResult(removeResult);
                            }
                        }

                        if (rolesToAdd.Length > 0)
                        {
                            var addResult =
                                await _userManager.AddToRolesAsync(user, rolesToAdd);

                            if (!addResult.Succeeded)
                            {
                                await transaction.RollbackAsync();
                                return ServiceResult.FromIdentityResult(addResult);
                            }
                        }

                        var stampResult =
                            await _userManager.UpdateSecurityStampAsync(user);

                        if (!stampResult.Succeeded)
                        {
                            await transaction.RollbackAsync();
                            return ServiceResult.FromIdentityResult(stampResult);
                        }

                        await transaction.CommitAsync();

                        return ServiceResult.Success();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                },
                verifySucceeded: null,
                cancellationToken: CancellationToken.None);
        }

        public async Task<ServiceResult> DeleteAsync(string id, string currentUserId)
        {
            if (string.Equals(id, currentUserId, StringComparison.Ordinal))
                return ServiceResult.Fail("User_CannotDeleteYourself");

            var strategy = _uow.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _uow.BeginTransactionAsync();

                try
                {
                    var user = await _userManager.FindByIdAsync(id);

                    if (user is null)
                        return ServiceResult.Fail("User_NotFound");

                    if (await _userManager.IsInRoleAsync(user, AppRoles.SuperAdmin.ToString()))
                        return ServiceResult.Fail("User_CannotDeleteSuperAdmin");

                    var permissions = await _uow.UserPermissionRepository.FindAsync(up => up.UserId == id);
                    foreach (var permission in permissions)
                    {
                        _uow.UserPermissionRepository.Delete(permission);
                    }
                    await _uow.SaveChangesAsync();

                    var result = await _userManager.DeleteAsync(user);

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

        public async Task<ServiceResult> SetLockoutAsync(string userId, int? durationMinutes)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return ServiceResult.Fail("User_NotFound");

            if (durationMinutes.HasValue)
            {
                if (await _userManager.IsInRoleAsync(user, AppRoles.SuperAdmin.ToString()))
                    return ServiceResult.Fail("User_CannotLockSuperAdmin");

                if (!user.LockoutEnabled)
                {
                    var enableResult = await _userManager.SetLockoutEnabledAsync(user, true);
                    if (!enableResult.Succeeded)
                        return ServiceResult.FromIdentityResult(enableResult);
                }

                var lockoutEnd = durationMinutes.Value <= 0
                    ? DateTimeOffset.MaxValue
                    : DateTimeOffset.UtcNow.AddMinutes(durationMinutes.Value);

                var lockResult = await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);
                if (!lockResult.Succeeded)
                    return ServiceResult.FromIdentityResult(lockResult);

                var stampResult = await _userManager.UpdateSecurityStampAsync(user);
                if (!stampResult.Succeeded)
                    return ServiceResult.FromIdentityResult(stampResult);
            }
            else
            {
                var unlockResult = await _userManager.SetLockoutEndDateAsync(user, null);
                if (!unlockResult.Succeeded)
                    return ServiceResult.FromIdentityResult(unlockResult);

                var resetResult = await _userManager.ResetAccessFailedCountAsync(user);
                if (!resetResult.Succeeded)
                    return ServiceResult.FromIdentityResult(resetResult);
            }

            return ServiceResult.Success();
        }
    }
}
