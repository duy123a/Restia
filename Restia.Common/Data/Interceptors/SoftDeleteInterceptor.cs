using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Restia.Common.Data.Constants;
using Restia.Common.Entities.Interfaces;
using System.Security.Claims;

namespace Restia.Common.Data.Interceptors
{
    public class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SoftDeleteInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            ApplySoftDelete(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            ApplySoftDelete(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void ApplySoftDelete(DbContext? context)
        {
            if (context == null)
            {
                return;
            }

            var now = DateTimeOffset.UtcNow;
            var currentUser =
                _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? DataConstants.SystemName;

            foreach (var entry in context.ChangeTracker.Entries<ISoftDeletable>())
            {
                if (entry.State != EntityState.Deleted)
                {
                    continue;
                }

                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = now;
                entry.Entity.DeletedBy = currentUser;
            }
        }
    }
}
