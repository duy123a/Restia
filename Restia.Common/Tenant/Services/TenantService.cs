using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Restia.Common.Form.Services;
using Restia.Common.Identity.Configurations;
using Restia.Common.Identity.Entities;
using Restia.Common.Identity.Enums;
using Restia.Common.Identity.Services;
using Restia.Common.Shared.Models;
using Restia.Common.Shared.UnitOfWorks.Interfaces;
using Restia.Common.Tenant.Constants;
using Restia.Common.Tenant.Models;
using Restia.Common.Tenant.Services.Interfaces;
using TenantEntity = Restia.Common.Tenant.Entities.Tenant;

namespace Restia.Common.Tenant.Services
{
    public class TenantService : ITenantService
    {
        private readonly IAdminUnitOfWork _uow;
        private readonly UserManager<AppUser> _userManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMemoryCache _memoryCache;
        private readonly SeedAdminSettings _seedOptions;

        public TenantService(
            IAdminUnitOfWork uow,
            UserManager<AppUser> userManager,
            IServiceProvider serviceProvider,
            IMemoryCache memoryCache,
            IOptions<SeedAdminSettings> seedOptions)
        {
            _uow = uow;
            _userManager = userManager;
            _serviceProvider = serviceProvider;
            _memoryCache = memoryCache;
            _seedOptions = seedOptions.Value;
        }

        public Task<PagedResult<TenantGetDto>> GetPagedListAsync(
            SearchParam? searchParam,
            int page,
            int pageSize)
        {
            return _uow.TenantRepository.GetPagedListAsync(searchParam, page, pageSize);
        }

        public async Task<TenantUpdateDto?> GetInfoForUpdateAsync(string id)
        {
            var tenant = await _uow.TenantRepository.GetByIdAsync(id);
            if (tenant == null)
            {
                return null;
            }

            return new TenantUpdateDto
            {
                Id = tenant.Id,
                DisplayName = tenant.DisplayName,
                IsActive = tenant.IsActive
            };
        }

        public async Task<ServiceResult> CreateAsync(TenantCreateDto dto)
        {
            var id = dto.Id.ToLowerInvariant();
            if (!TenantConstants.IsValidSchemaName(id))
            {
                return ServiceResult.Fail("Tenant_InvalidId");
            }

            if (await _uow.TenantRepository.ExistsAnyAsync(id))
            {
                return ServiceResult.Fail("Tenant_DuplicateId");
            }

            // Pre-validate SuperAdmin password before provisioning PostgreSQL schema
            var candidateAdmin = new AppUser
            {
                UserName = dto.AdminUserName,
                Email = dto.AdminEmail,
                DisplayName = AppRoles.SuperAdmin.ToString(),
                SchemaId = id,
                EmailConfirmed = true
            };

            foreach (var passwordValidator in _userManager.PasswordValidators)
            {
                var pwdCheck = await passwordValidator.ValidateAsync(_userManager, candidateAdmin, dto.AdminPassword);
                if (!pwdCheck.Succeeded)
                {
                    return ServiceResult.FromIdentityResult(pwdCheck);
                }
            }

            await _uow.TenantRepository.EnsureSchemaCreatedAsync(id);

            var provisionResult = await ProvisionTenantSchemaAsync(
                id,
                dto.AdminUserName,
                dto.AdminEmail,
                dto.AdminPassword);

            if (!provisionResult.Succeeded)
            {
                return provisionResult;
            }

            var tenant = new TenantEntity
            {
                Id = id,
                DisplayName = dto.DisplayName,
                IsActive = dto.IsActive
            };

            await _uow.TenantRepository.AddAsync(tenant);
            await _uow.SaveChangesAsync();

            _memoryCache.Remove(TenantConstants.ActiveTenantsCacheKey);

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> UpdateAsync(TenantUpdateDto dto)
        {
            var tenant = await _uow.TenantRepository.GetByIdAsync(dto.Id);
            if (tenant == null)
            {
                return ServiceResult.Fail("Tenant_NotFound");
            }

            var isPublic = string.Equals(tenant.Id, TenantConstants.DefaultTenantId, StringComparison.OrdinalIgnoreCase);
            if (isPublic && !dto.IsActive)
            {
                return ServiceResult.Fail("Tenant_CannotDeactivatePublic");
            }

            tenant.DisplayName = dto.DisplayName;
            tenant.IsActive = isPublic || dto.IsActive;

            _uow.TenantRepository.Update(tenant);
            await _uow.SaveChangesAsync();

            _memoryCache.Remove(TenantConstants.ActiveTenantsCacheKey);

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeleteAsync(string id)
        {
            if (string.Equals(id, TenantConstants.DefaultTenantId, StringComparison.OrdinalIgnoreCase))
            {
                return ServiceResult.Fail("Tenant_CannotDeletePublic");
            }

            var tenant = await _uow.TenantRepository.GetByIdAsync(id);
            if (tenant == null)
            {
                return ServiceResult.Fail("Tenant_NotFound");
            }

            _uow.TenantRepository.Delete(tenant);
            await _uow.SaveChangesAsync();

            _memoryCache.Remove(TenantConstants.ActiveTenantsCacheKey);

            return ServiceResult.Success();
        }

        public async Task<bool> IsValidActiveTenantAsync(string? schemaId)
        {
            if (!TenantConstants.IsValidSchemaName(schemaId))
            {
                return false;
            }

            var activeTenants = await _memoryCache.GetOrCreateAsync(
                TenantConstants.ActiveTenantsCacheKey,
                async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

                    using var scope = _serviceProvider.CreateScope();
                    var publicUow = scope.ServiceProvider.GetRequiredService<IAdminUnitOfWork>();
                    var tenants = await publicUow.TenantRepository.FindAsync(t => t.IsActive);

                    var set = tenants
                        .Select(t => t.Id)
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    set.Add(TenantConstants.DefaultTenantId);
                    return set;
                });

            return activeTenants != null && activeTenants.Contains(schemaId!);
        }

        public async Task InitializeAllTenantsAsync(CancellationToken cancellationToken = default)
        {
            // 1. Migrate & seed default 'public' schema
            using (var publicScope = _serviceProvider.CreateScope())
            {
                var publicUow = publicScope.ServiceProvider.GetRequiredService<IAdminUnitOfWork>();
                var publicRoleManager = publicScope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
                var publicUserManager = publicScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                await publicUow.TenantRepository.MigrateAsync(cancellationToken);

                var publicTenant = await publicUow.TenantRepository.GetByIdAsync(TenantConstants.DefaultTenantId);
                if (publicTenant == null)
                {
                    await publicUow.TenantRepository.AddAsync(new TenantEntity
                    {
                        Id = TenantConstants.DefaultTenantId,
                        DisplayName = "Public (Main)",
                        IsActive = true
                    }, cancellationToken);
                    await publicUow.SaveChangesAsync(cancellationToken);
                }

                await FormSeedWorker.SeedFormsAsync(publicUow, cancellationToken);
                await IdentitySeedWorker.SeedRolesAsync(publicRoleManager);

                if (!string.IsNullOrWhiteSpace(_seedOptions.AdminEmail) &&
                    !string.IsNullOrWhiteSpace(_seedOptions.AdminPassword))
                {
                    var admin = await publicUserManager.FindByEmailAsync(_seedOptions.AdminEmail);
                    if (admin == null)
                    {
                        admin = new AppUser
                        {
                            Email = _seedOptions.AdminEmail,
                            UserName = _seedOptions.AdminEmail.Split('@')[0],
                            EmailConfirmed = true,
                            DisplayName = AppRoles.SuperAdmin.ToString(),
                            SchemaId = TenantConstants.DefaultTenantId
                        };

                        var createResult = await publicUserManager.CreateAsync(admin, _seedOptions.AdminPassword);
                        if (createResult.Succeeded)
                        {
                            await publicUserManager.AddToRoleAsync(admin, AppRoles.SuperAdmin.ToString());
                        }
                    }
                }

                // 2. Migrate & seed all active non-public tenant schemas
                var activeTenants = await publicUow.TenantRepository.FindAsync(
                    t => t.IsActive && t.Id != TenantConstants.DefaultTenantId,
                    cancellationToken);

                foreach (var tenant in activeTenants)
                {
                    if (!TenantConstants.IsValidSchemaName(tenant.Id))
                    {
                        continue;
                    }

                    await publicUow.TenantRepository.EnsureSchemaCreatedAsync(tenant.Id, cancellationToken);

                    using var tenantScope = _serviceProvider.CreateScope();
                    var tenantContext = tenantScope.ServiceProvider.GetRequiredService<ITenantContext>();
                    tenantContext.SetTenant(tenant.Id);

                    var tenantUow = tenantScope.ServiceProvider.GetRequiredService<IAdminUnitOfWork>();
                    var tenantRoleManager = tenantScope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

                    await tenantUow.TenantRepository.MigrateAsync(cancellationToken);
                    await FormSeedWorker.SeedFormsAsync(tenantUow, cancellationToken);
                    await IdentitySeedWorker.SeedRolesAsync(tenantRoleManager);
                }
            }

            _memoryCache.Remove(TenantConstants.ActiveTenantsCacheKey);
        }

        private async Task<ServiceResult> ProvisionTenantSchemaAsync(
            string schemaId,
            string adminUserName,
            string adminEmail,
            string adminPassword)
        {
            using var tenantScope = _serviceProvider.CreateScope();
            var tenantContext = tenantScope.ServiceProvider.GetRequiredService<ITenantContext>();
            tenantContext.SetTenant(schemaId);

            var tenantUow = tenantScope.ServiceProvider.GetRequiredService<IAdminUnitOfWork>();
            var tenantRoleManager = tenantScope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
            var tenantUserManager = tenantScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            await tenantUow.TenantRepository.MigrateAsync();
            await FormSeedWorker.SeedFormsAsync(tenantUow);
            await IdentitySeedWorker.SeedRolesAsync(tenantRoleManager);

            var existingByEmail = await tenantUserManager.FindByEmailAsync(adminEmail);
            var existingByName = await tenantUserManager.FindByNameAsync(adminUserName);
            if (existingByEmail == null && existingByName == null)
            {
                var admin = new AppUser
                {
                    UserName = adminUserName,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    DisplayName = AppRoles.SuperAdmin.ToString(),
                    SchemaId = schemaId
                };

                var createResult = await tenantUserManager.CreateAsync(admin, adminPassword);
                if (!createResult.Succeeded)
                {
                    return ServiceResult.FromIdentityResult(createResult);
                }

                var roleResult = await tenantUserManager.AddToRoleAsync(admin, AppRoles.SuperAdmin.ToString());
                if (!roleResult.Succeeded)
                {
                    return ServiceResult.FromIdentityResult(roleResult);
                }
            }

            return ServiceResult.Success();
        }
    }
}
