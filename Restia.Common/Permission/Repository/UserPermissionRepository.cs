using Restia.Common.Data.Context;
using Restia.Common.Permission.Entities;
using Restia.Common.Permission.Repository.Interfaces;
using Restia.Common.Shared.Repository;

namespace Restia.Common.Permission.Repository
{
    public class UserPermissionRepository : BaseRepository<UserPermission>, IUserPermissionRepository
    {
        public UserPermissionRepository(AppDbContext context) : base(context)
        {
        }
    }
}
