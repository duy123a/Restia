using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Restia.Common.Identity.Configurations;
using Restia.Common.Identity.Entities;
using Restia.Common.Localization.Resources;
using Restia.Common.Shared.Helpers;
using Restia.Web.Models;

namespace Restia.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly CookieSettings _cookieSettings;

        public AccountController(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IStringLocalizer<SharedResources> localizer,
            IOptions<CookieSettings> cookieSettings)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _localizer = localizer;
            _cookieSettings = cookieSettings.Value;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel
            {
                ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) || returnUrl == "/"
                    ? Url.Action("Index", "Home")
                    : returnUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ReturnUrl) || model.ReturnUrl == "/")
            {
                model.ReturnUrl = Url.Action("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = model.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(_cookieSettings.RememberMeExpireDays)
                    : DateTimeOffset.UtcNow.AddSeconds(_cookieSettings.DefaultExpireSeconds)
            };

            var loginInfo = model.LoginInfo ?? string.Empty;
            var user = await _userManager.FindByEmailAsync(loginInfo)
                ?? await _userManager.FindByNameAsync(loginInfo);

            if (user != null)
            {
                var result = await _signInManager.CheckPasswordSignInAsync(
                    user,
                    model.Password,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, authProperties);

                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                if (result.IsLockedOut)
                {
                    var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                    if (lockoutEnd.HasValue && lockoutEnd.Value >= DateTimeOffset.MaxValue.AddYears(-1))
                    {
                        ModelState.AddModelError(string.Empty, _localizer["Login_LockedOutPermanent"]);
                    }
                    else if (lockoutEnd.HasValue)
                    {
                        var unlockTime = lockoutEnd.Value.ToAppTime().ToString("dd/MM/yyyy HH:mm:ss");
                        ModelState.AddModelError(string.Empty, _localizer["Login_LockedOutUntil", unlockTime]);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, _localizer["Login_Failed"]);
                    }
                    return View(model);
                }
            }

            ModelState.AddModelError(string.Empty, _localizer["Login_Failed"]);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string? returnUrl = null)
        {
            if (!(User.Identity?.IsAuthenticated ?? false))
            {
                return RedirectToAction("Index", "Home");
            }

            await _signInManager.SignOutAsync();

            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
    }
}
