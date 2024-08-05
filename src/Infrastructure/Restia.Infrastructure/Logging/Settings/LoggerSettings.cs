namespace Restia.Infrastructure.Logging.Settings;
/// <summary>
/// Logger settings
/// </summary>
public class LoggerSettings
{
	/// <summary>Get and set application name</summary>
	public string AppName { get; set; } = "Restia.WebApi";
	/// <summary>Get and set write to file</summary>
	public bool WriteToFile { get; set; }
	/// <summary>Get and set output log file path</summary>
	public string OutputLogFilePath { get; set; } = "~";
	/// <summary>Get and set structured console logging</summary>
	public bool StructuredConsoleLogging { get; set; }
	/// <summary>Get and set minimum log level</summary>
	public string MinimumLogLevel { get; set; } = "Information";
}
