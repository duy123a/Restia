using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using OrchardCore.Localization;
using Restia.Infrastructure.Common.Extensions;

namespace Restia.Infrastructure.Localization.Services;

public class RestiaPoFileLocationProvider : ILocalizationFileLocationProvider
{
	/// <summary>File provider</summary>
	private readonly IFileProvider _fileProvider;
	/// <summary>Resources container</summary>
	private readonly string _resourcesContainer;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="hostingEnvironment">The hosting environment</param>
	/// <param name="localizationOptions">The localization options</param>
	public RestiaPoFileLocationProvider(
		IHostEnvironment hostingEnvironment,
		IOptions<LocalizationOptions> localizationOptions)
	{
		_fileProvider = hostingEnvironment.ContentRootFileProvider;
		_resourcesContainer = localizationOptions.Value.ResourcesPath;
	}

	/// <summary>
	/// Get locations
	/// </summary>
	/// <param name="cultureName">The culture name</param>
	/// <returns>A list of <see cref="IFileInfo"/>.</returns>
	public IEnumerable<IFileInfo> GetLocations(string cultureName)
	{
		// Loads all *.po files from the culture folder under the Resource Path.
		// for example in this case, it will load from src\Host\Restia.WebApi\Localization
		foreach (var file in _fileProvider.GetDirectoryContents(PathExtensions.Combine(_resourcesContainer, cultureName)))
		{
			yield return file;
		}
	}
}
