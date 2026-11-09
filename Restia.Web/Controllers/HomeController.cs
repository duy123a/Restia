using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restia.Common.Identity.Extensions;
using Restia.Common.Notification.Service.Interfaces;

namespace Restia.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IAppNotificationService _notificationService;

        public HomeController(IAppNotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NotifyDemo()
        {
            var userId = User.GetUserId();
            await _notificationService.NotifyDemoAsync(userId);

            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return Ok();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NotifyAdminDemo()
        {
            await _notificationService.NotifyAdminDemoAsync();

            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return Ok();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
