using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Restia.Common.Localization.Constants;
using System.Globalization;

namespace Restia.Common.Localization.Extensions
{
    public static class LocalizationExtensions
    {
        public static IServiceCollection AddLocalizationModule(this IServiceCollection services)
        {
            var cultures = LocalizationConstants.All.Select(c => new CultureInfo(c)).ToList();

            services.AddLocalization();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(LocalizationConstants.Default);
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;

                options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new QueryStringRequestCultureProvider(),   // ?culture=vi
                    new CookieRequestCultureProvider
                    {
                        CookieName = CookieRequestCultureProvider.DefaultCookieName
                    },
                    new AcceptLanguageHeaderRequestCultureProvider() // Accept-Language: vi-VN
                };
            });

            return services;
        }
    }
}
