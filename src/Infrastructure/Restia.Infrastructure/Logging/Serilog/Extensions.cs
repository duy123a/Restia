using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Restia.Infrastructure.Logging.Settings;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;

namespace Restia.Infrastructure.Logging.Serilog;

/// <summary>
/// Extensions
/// </summary>
public static class Extensions
{
	/// <summary>
	/// Register serilog
	/// </summary>
	/// <param name="builder">The builder</param>
	public static void RegisterSerilog(this WebApplicationBuilder builder)
	{
		// You can get this option by IOptions<LoggerSettings>
		builder.Services.AddOptions<LoggerSettings>().BindConfiguration(nameof(LoggerSettings));

		// serilogConfig is initialized internally by Serilog
		// You can get this log by ILogger<T> where T is your controller class
		_ = builder.Host.UseSerilog((_, sp, serilogConfig) =>
		{
			var loggerSettings = sp.GetRequiredService<IOptions<LoggerSettings>>().Value;
			var appName = loggerSettings.AppName;
			var writeToFile = loggerSettings.WriteToFile;
			var outputLogFilePath = $"{loggerSettings.OutputLogFilePath}/{appName}/";
			var structuredConsoleLogging = loggerSettings.StructuredConsoleLogging;
			var minLogLevel = loggerSettings.MinimumLogLevel;
			ConfigureEnrichers(serilogConfig, appName);
			ConfigureConsoleLogging(serilogConfig, structuredConsoleLogging);
			ConfigureWriteToFile(serilogConfig, writeToFile, outputLogFilePath);
			SetMinimumLogLevel(serilogConfig, minLogLevel);
			OverideMinimumLogLevel(serilogConfig);
		});
	}

	/// <summary>
	/// Configure enrichers
	/// </summary>
	/// <param name="serilogConfig">The serilog configuration</param>
	/// <param name="appName">The application name</param>
	private static void ConfigureEnrichers(LoggerConfiguration serilogConfig, string appName)
	{
		serilogConfig
			.Enrich.FromLogContext()
			.Enrich.WithProperty("Application", appName)
			.Enrich.WithExceptionDetails()
			.Enrich.WithMachineName()
			.Enrich.WithProcessId()
			.Enrich.WithThreadId();
	}

	/// <summary>
	/// Configure console logging
	/// </summary>
	/// <param name="serilogConfig">The serilog configuration</param>
	/// <param name="structuredConsoleLogging">The structured console logging</param>
	private static void ConfigureConsoleLogging(LoggerConfiguration serilogConfig, bool structuredConsoleLogging)
	{
		if (structuredConsoleLogging)
		{
			//{
			//	"Timestamp": "2024-08-01T12:34:56.789Z",
			//	  "Level": "Information",
			//	  "MessageTemplate": "This is an informational message.",
			//	  "Properties": {
			//		"Application": "MyApp",
			//		"MachineName": "MyMachine",
			//		"ProcessId": 1234,
			//		"ThreadId": 5,
			//		"SourceContext": "MyNamespace.MyController"
			//	  }
			//}
			serilogConfig.WriteTo.Async(wt => wt.Console(new CompactJsonFormatter()));
		}
		else
		{
			// [12:34:56 INF] This is an informational message.
			serilogConfig.WriteTo.Async(wt => wt.Console());
		}
	}

	/// <summary>
	/// Configure write to file
	/// </summary>
	/// <param name="serilogConfig">The serilog configuration</param>
	/// <param name="isWriteToFile">Is write to file</param>
	/// <param name="outputLogFilePath">Output log file path</param>
	private static void ConfigureWriteToFile(
		LoggerConfiguration serilogConfig,
		bool isWriteToFile,
		string outputLogFilePath)
	{
		if (isWriteToFile)
		{
			// This allow write log base on log event level
			serilogConfig.WriteTo.Map(
				evt => evt.Level,
				(level, wt) => wt.File(
					new CompactJsonFormatter(),
					$"{outputLogFilePath}{level.ToString().ToLower()}_logs.json",
					restrictedToMinimumLevel: LogEventLevel.Information,
					rollingInterval: RollingInterval.Day,
					retainedFileCountLimit: 5));
		}
	}

	/// <summary>
	/// Set minimum log level
	/// </summary>
	/// <param name="serilogConfig">The serilog configuration</param>
	/// <param name="minLogLevel">The min log level</param>
	private static void SetMinimumLogLevel(LoggerConfiguration serilogConfig, string minLogLevel)
	{
		switch (minLogLevel.ToLower())
		{
			case "debug":
				serilogConfig.MinimumLevel.Debug();
				break;

			case "information":
				serilogConfig.MinimumLevel.Information();
				break;

			case "warning":
				serilogConfig.MinimumLevel.Warning();
				break;

			default:
				serilogConfig.MinimumLevel.Information();
				break;
		}
	}

	/// <summary>
	/// Overide minimum log level
	/// </summary>
	/// <param name="serilogConfig">The serilog configuration</param>
	private static void OverideMinimumLogLevel(LoggerConfiguration serilogConfig)
	{
		serilogConfig
			.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
			.MinimumLevel.Override("Hangfire", LogEventLevel.Warning)
			.MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
			.MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error);
	}
}
