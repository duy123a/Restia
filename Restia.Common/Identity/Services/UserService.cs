using Microsoft.AspNetCore.Identity;
using Restia.Common.Identity.Models;
using Restia.Common.Identity.Services.Interfaces;
using Restia.Common.Shared.Entities;
using Restia.Common.Shared.Models;
using Restia.Common.Shared.UnitOfWorks.Interfaces;

namespace Restia.Common.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IAdminUnitOfWork _uow;

        public UserService(
            UserManager<AppUser> userManager,
            IAdminUnitOfWork uow)
        {
            _userManager = userManager;
            _uow = uow;
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

            return new UserUpdateDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                DisplayName = user.DisplayName,
                PhoneNumber = user.PhoneNumber,
                SelectedRoles = selectedRoles.ToList(),
                AvailableRoles = availableRoles
            };
        }

        public Task<IReadOnlyList<RoleOptionDto>> GetAllRoleOptionsAsync()
            => _uow.RoleRepository.GetAllRoleOptionsAsync();

        public async Task<ServiceResult> CreateAsync(UserCreateDto dto)
        {
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

                        user.UserName = dto.UserName;
                        user.Email = dto.Email;
                        user.PhoneNumber = dto.PhoneNumber;
                        user.DisplayName = dto.DisplayName;

                        var updateResult = await _userManager.UpdateAsync(user);

                        if (!updateResult.Succeeded)
                            return ServiceResult.FromIdentityResult(updateResult);

                        if (!string.IsNullOrWhiteSpace(dto.Password))
                        {
                            var token =
                                await _userManager.GeneratePasswordResetTokenAsync(user);

                            var passwordResult =
                                await _userManager.ResetPasswordAsync(
                                    user, token, dto.Password);

                            if (!passwordResult.Succeeded)
                                return ServiceResult.FromIdentityResult(passwordResult);
                        }

                        var currentRoles = await _userManager.GetRolesAsync(user);

                        var rolesToRemove = currentRoles
                            .Except(dto.SelectedRoles, StringComparer.OrdinalIgnoreCase)
                            .ToArray();

                        var rolesToAdd = dto.SelectedRoles
                            .Except(currentRoles, StringComparer.OrdinalIgnoreCase)
                            .ToArray();

                        if (rolesToRemove.Length > 0)
                        {
                            var removeResult =
                                await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

                            if (!removeResult.Succeeded)
                                return ServiceResult.FromIdentityResult(removeResult);
                        }

                        if (rolesToAdd.Length > 0)
                        {
                            var addResult =
                                await _userManager.AddToRolesAsync(user, rolesToAdd);

                            if (!addResult.Succeeded)
                                return ServiceResult.FromIdentityResult(addResult);
                        }

                        var stampResult =
                            await _userManager.UpdateSecurityStampAsync(user);

                        if (!stampResult.Succeeded)
                            return ServiceResult.FromIdentityResult(stampResult);

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

            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return ServiceResult.Fail("User_NotFound");

            var result = await _userManager.DeleteAsync(user);

            return ServiceResult.FromIdentityResult(result);
        }
    }
}
