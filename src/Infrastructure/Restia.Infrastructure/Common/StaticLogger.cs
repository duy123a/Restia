using Serilog;

namespace Restia.Infrastructure.Common;

/// <summary>
/// Static logger
/// </summary>
public static class StaticLogger
{
	/// <summary>
	/// Ensure initialize logger
	/// </summary>
	public static void EnsureInitialized()
	{
		if (Log.Logger is not Serilog.Core.Logger)
		{
			Log.Logger = new LoggerConfiguration()
				.Enrich.FromLogContext()
				.WriteTo.Console()
				.CreateLogger();
		}
	}
}
