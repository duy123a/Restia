using Restia.Common.Data.Context;
using Restia.Common.Permission.Entities;
using Restia.Common.Permission.Repository.Interfaces;
using Restia.Common.Shared.Repository;

namespace Restia.Common.Permission.Repository
{
    public class RolePermissionRepository : BaseRepository<RolePermission>, IRolePermissionRepository
    {
        public RolePermissionRepository(AppDbContext context) : base(context)
        {
        }
    }
}
