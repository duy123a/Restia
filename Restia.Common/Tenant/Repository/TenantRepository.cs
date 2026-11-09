using Microsoft.EntityFrameworkCore;
using Restia.Common.Data.Context;
using Restia.Common.Shared.Constants;
using Restia.Common.Shared.Models;
using Restia.Common.Shared.Repository;
using Restia.Common.Tenant.Models;
using Restia.Common.Tenant.Repository.Interfaces;
using TenantEntity = Restia.Common.Tenant.Entities.Tenant;

namespace Restia.Common.Tenant.Repository
{
    public class TenantRepository : BaseRepository<TenantEntity>, ITenantRepository
    {
        public TenantRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<TenantGetDto>> GetPagedListAsync(
            SearchParam? searchParam,
            int page,
            int pageSize)
        {
            page = Math.Max(1, page);
            pageSize = Math.Max(1, pageSize);
            var query = DbSet.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchParam?.SearchTerm))
            {
                var pattern = $"%{searchParam.SearchTerm}%";
                query = query.Where(t =>
                    EF.Functions.ILike(t.Id, pattern) ||
                    EF.Functions.ILike(t.DisplayName, pattern));
            }

            var totalCount = await query.CountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
            if (page > totalPages)
            {
                page = totalPages;
            }

            query = searchParam?.OrderBy switch
            {
                AppConstants.NameDesc => query.OrderByDescending(t => t.DisplayName).ThenBy(t => t.Id),
                AppConstants.NameAsc => query.OrderBy(t => t.DisplayName).ThenBy(t => t.Id),
                _ => query.OrderBy(t => t.Id)
            };

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TenantGetDto(
                    t.Id,
                    t.DisplayName,
                    t.IsActive,
                    t.CreatedAt))
                .ToListAsync();

            return new PagedResult<TenantGetDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public Task<bool> ExistsAnyAsync(string id, CancellationToken cancellationToken = default)
        {
            return DbSet
                .IgnoreQueryFilters()
                .AnyAsync(t => t.Id == id, cancellationToken);
        }

        public Task EnsureSchemaCreatedAsync(string schemaId, CancellationToken cancellationToken = default)
        {
            var createSchemaSql = string.Concat("CREATE SCHEMA IF NOT EXISTS \"", schemaId, "\";");
            return Context.Database.ExecuteSqlRawAsync(createSchemaSql, cancellationToken);
        }

        public Task MigrateAsync(CancellationToken cancellationToken = default)
        {
            return Context.Database.MigrateAsync(cancellationToken);
        }
    }
}
