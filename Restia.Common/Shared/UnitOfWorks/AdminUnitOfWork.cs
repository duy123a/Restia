using Restia.Common.Data.Context;
using Restia.Common.Form.Repository.Interfaces;
using Restia.Common.Identity.Repository.Interfaces;
using Restia.Common.Permission.Repository.Interfaces;
using Restia.Common.Shared.Repository;
using Restia.Common.Shared.UnitOfWorks.Interfaces;

namespace Restia.Common.Shared.UnitOfWorks
{
    public class AdminUnitOfWork : BaseUnitOfWork, IAdminUnitOfWork
    {
        private IFormRepository _formRepository;
        private IUserPermissionRepository _userPermissionRepository;
        private IRolePermissionRepository _rolePermissionRepository;
        private IUserRepository _userRepository;
        private IRoleRepository _roleRepository;

        public AdminUnitOfWork(
            AppDbContext context,
            IFormRepository formRepository,
            IUserPermissionRepository userPermissionRepository,
            IRolePermissionRepository rolePermissionRepository,
            IUserRepository userRepository,
            IRoleRepository roleRepository) : base(context)
        {
            _formRepository = formRepository;
            _userPermissionRepository = userPermissionRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public IFormRepository FormRepository => _formRepository;

        public IUserPermissionRepository UserPermissionRepository => _userPermissionRepository;

        public IRolePermissionRepository RolePermissionRepository => _rolePermissionRepository;

        public IUserRepository UserRepository => _userRepository;

        public IRoleRepository RoleRepository => _roleRepository;
    }
}
