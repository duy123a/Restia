using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Restia.Common.Localization.Constants;

namespace Restia.Web.Controllers
{
    public class CultureController : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            var selectedCulture = culture.ToLower().Equals(LocalizationConstants.Vietnamese, StringComparison.OrdinalIgnoreCase)
                ? LocalizationConstants.Vietnamese
                : LocalizationConstants.English;

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(selectedCulture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    Secure = true,
                    HttpOnly = true
                }
            );

            return LocalRedirect(returnUrl ?? "/");
        }
    }
}
