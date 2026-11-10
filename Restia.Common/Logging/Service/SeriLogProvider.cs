using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;

namespace Restia.Common.Logging.Service
{
    public static class SeriLogProvider
    {
        private const string _template =
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] | " +
            "System={SystemName} | " +
            "Server={ServerName} | " +
            "ProcessId={ProcessId} | " +
            "Version={Version} | " +
            "TenantId={TenantId} | " +
            "UserId={UserId} | " +
            "ClientIp={ClientIp} | " +
            "RequestId={RequestId} | " +
            "Method={Method} | " +
            "Message={Message:lj}{NewLine}{Exception}";

        public static void Initialize(
            IHostEnvironment env,
            ILogEventEnricher eventEnricher)
        {
            var logConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.With(eventEnricher);

            if (env.IsDevelopment())
            {
                logConfiguration.WriteTo.Console(
                    outputTemplate: _template);
            }
            else
            {
                var logPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Logs",
                    "log.txt");

                Directory.CreateDirectory(
                    Path.GetDirectoryName(logPath)!);

                logConfiguration
                    .WriteTo.File(
                        logPath,
                        outputTemplate: _template,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 30);
            }

            Log.Logger = logConfiguration.CreateLogger();
        }
    }
}
