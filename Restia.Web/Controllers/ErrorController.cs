using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Restia.Common.Localization.Resources;
using Restia.Web.Models;
using System.Diagnostics;

namespace Restia.Web.Controllers
{
    public class ErrorController : Controller
    {
        private readonly IStringLocalizer<SharedResources> _localizer;

        public ErrorController(IStringLocalizer<SharedResources> localizer)
        {
            _localizer = localizer;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("Error/{statusCode?}")]
        public IActionResult Index(int? statusCode = null)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var code = statusCode ?? (HttpContext.Response.StatusCode >= 400 ? HttpContext.Response.StatusCode : null);

            var (title, message) = code switch
            {
                400 => (_localizer["Error_400_Title"].Value, _localizer["Error_400"].Value),
                401 => (_localizer["Error_401_Title"].Value, _localizer["Error_401"].Value),
                403 => (_localizer["Error_403_Title"].Value, _localizer["Error_403"].Value),
                404 => (_localizer["Error_404_Title"].Value, _localizer["Error_404"].Value),
                500 => (_localizer["Error_500_Title"].Value, _localizer["Error_500"].Value),
                _ => (_localizer["Error_Title"].Value, _localizer["Error_Default"].Value)
            };

            var viewModel = new ErrorViewModel
            {
                RequestId = requestId,
                StatusCode = code,
                Title = title,
                ErrorMessage = message,
                Timestamp = DateTimeOffset.UtcNow
            };

            return View("Error", viewModel);
        }
    }
}
