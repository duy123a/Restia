using Restia.Common.Form.Models;
using Restia.Common.Form.Repository.Interfaces;
using Restia.Common.Form.Services.Interfaces;
using Restia.Common.Permission.Services.Interfaces;
using System.Security.Claims;
using FormEntity = Restia.Common.Form.Entities.Form;

namespace Restia.Common.Form.Services
{
    public class FormService : IFormService
    {
        private readonly IFormRepository _formRepository;
        private readonly IPermissionChecker _permissionChecker;

        public FormService(
            IFormRepository formRepository,
            IPermissionChecker permissionChecker)
        {
            _formRepository = formRepository;
            _permissionChecker = permissionChecker;
        }

        public async Task<IReadOnlyList<FormItemDto>> GetSidebarMenuAsync(
            ClaimsPrincipal user)
        {
            var roots = await _formRepository
                .GetActiveRootMenusWithChildrenAsync();

            var result = new List<FormItemDto>();

            foreach (var root in roots)
            {
                var dto = await MapMenuAsync(root, user);

                if (dto != null)
                {
                    result.Add(dto);
                }
            }

            return result;
        }

        private async Task<FormItemDto?> MapMenuAsync(
            FormEntity menu,
            ClaimsPrincipal user)
        {
            var children = new List<FormItemDto>();

            foreach (var child in menu.Children.OrderBy(c => c.DisplayOrder))
            {
                var dto = await MapLeafAsync(child, user);

                if (dto != null)
                {
                    children.Add(dto);
                }
            }

            // Form có URL: đây là một màn hình có thể truy cập trực tiếp.
            if (!string.IsNullOrWhiteSpace(menu.Url))
            {
                if (!await CanAccessFormAsync(menu, user))
                {
                    return null;
                }

                return new FormItemDto
                {
                    NameKey = menu.Name,
                    Url = menu.Url,
                    Icon = menu.Icon,
                    Children = children
                };
            }

            // Form cha không có URL: chỉ hiện nếu có con được phép truy cập.
            if (children.Count == 0)
            {
                return null;
            }

            return new FormItemDto
            {
                NameKey = menu.Name,
                Icon = menu.Icon,
                Children = children
            };
        }

        private async Task<FormItemDto?> MapLeafAsync(
            FormEntity menu,
            ClaimsPrincipal user)
        {
            if (string.IsNullOrWhiteSpace(menu.Url))
            {
                return null;
            }

            if (!await CanAccessFormAsync(menu, user))
            {
                return null;
            }

            return new FormItemDto
            {
                NameKey = menu.Name,
                Url = menu.Url,
                Icon = menu.Icon
            };
        }

        private async Task<bool> CanAccessFormAsync(
            FormEntity form,
            ClaimsPrincipal user)
        {
            // Thay đổi format policy theo quy ước thực tế của bạn.
            var policyName = $"Form:{form.Id}";

            return await _permissionChecker.HasPermissionAsync(
                user,
                policyName);
        }
    }
}