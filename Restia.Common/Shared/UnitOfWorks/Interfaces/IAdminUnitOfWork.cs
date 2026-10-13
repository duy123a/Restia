using Restia.Common.Form.Repository.Interfaces;
using Restia.Common.Identity.Repository.Interfaces;
using Restia.Common.Permission.Repository.Interfaces;
using Restia.Common.Shared.Repository.Interfaces;

namespace Restia.Common.Shared.UnitOfWorks.Interfaces
{
    public interface IAdminUnitOfWork : IUnitOfWork
    {
        IFormRepository FormRepository { get; }

        IUserPermissionRepository UserPermissionRepository { get; }

        IRolePermissionRepository RolePermissionRepository { get; }

        IUserRepository UserRepository { get; }

        IRoleRepository RoleRepository { get; }
    }
}
