using Microsoft.AspNetCore.Mvc;
using Restia.Common.Form.Services.Interfaces;

namespace Restia.Web.ViewComponents
{
    public class SidebarMenuViewComponent : ViewComponent
    {
        private readonly IFormService _menuService;

        public SidebarMenuViewComponent(IFormService menuService)
        {
            _menuService = menuService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = await _menuService.GetSidebarMenuAsync(HttpContext.User);
            return View(items);
        }
    }
}
