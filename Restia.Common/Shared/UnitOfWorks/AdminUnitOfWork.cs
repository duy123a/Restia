using Restia.Common.Data.Context;
using Restia.Common.Form.Repository.Interfaces;
using Restia.Common.Identity.Repository.Interfaces;
using Restia.Common.Notification.Repository.Interfaces;
using Restia.Common.Permission.Repository.Interfaces;
using Restia.Common.Shared.Repository;
using Restia.Common.Shared.UnitOfWorks.Interfaces;
using Restia.Common.Tenant.Repository.Interfaces;

namespace Restia.Common.Shared.UnitOfWorks
{
    public class AdminUnitOfWork : BaseUnitOfWork, IAdminUnitOfWork
    {
        private readonly IFormRepository _formRepository;
        private readonly IUserPermissionRepository _userPermissionRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly ITenantRepository _tenantRepository;

        public AdminUnitOfWork(
            AppDbContext context,
            IFormRepository formRepository,
            IUserPermissionRepository userPermissionRepository,
            IRolePermissionRepository rolePermissionRepository,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            INotificationRepository notificationRepository,
            ITenantRepository tenantRepository) : base(context)
        {
            _formRepository = formRepository;
            _userPermissionRepository = userPermissionRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _notificationRepository = notificationRepository;
            _tenantRepository = tenantRepository;
        }

        public IFormRepository FormRepository => _formRepository;

        public IUserPermissionRepository UserPermissionRepository => _userPermissionRepository;

        public IRolePermissionRepository RolePermissionRepository => _rolePermissionRepository;

        public IUserRepository UserRepository => _userRepository;

        public IRoleRepository RoleRepository => _roleRepository;

        public INotificationRepository NotificationRepository => _notificationRepository;

        public ITenantRepository TenantRepository => _tenantRepository;
    }
}
