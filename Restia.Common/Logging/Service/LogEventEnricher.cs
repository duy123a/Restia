using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Restia.Common.Identity.Extensions;
using Restia.Common.Tenant.Constants;
using Serilog.Core;
using Serilog.Events;

namespace Restia.Common.Logging.Service
{
    public class LogEventEnricher : ILogEventEnricher
    {
        private const string FallbackValue = "-";

        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly LogEventProperty _systemNameProp;
        private readonly LogEventProperty _serverNameProp;
        private readonly LogEventProperty _versionProp;
        private readonly LogEventProperty _processIdProp;

        public LogEventEnricher(
            IConfiguration cfg,
            IHttpContextAccessor? httpContextAccessor = null)
        {
            _httpContextAccessor = httpContextAccessor;

            var systemName = cfg["SystemName"] ?? FallbackValue;
            var serverName = cfg["ServerName"] ?? Environment.MachineName;
            var version = cfg["Version"] ?? FallbackValue;
            var processId = Environment.ProcessId;

            _systemNameProp = new LogEventProperty("SystemName", new ScalarValue(systemName));
            _serverNameProp = new LogEventProperty("ServerName", new ScalarValue(serverName));
            _versionProp = new LogEventProperty("Version", new ScalarValue(version));
            _processIdProp = new LogEventProperty("ProcessId", new ScalarValue(processId));
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(_systemNameProp);
            logEvent.AddPropertyIfAbsent(_serverNameProp);
            logEvent.AddPropertyIfAbsent(_versionProp);
            logEvent.AddPropertyIfAbsent(_processIdProp);

            var httpContext = _httpContextAccessor?.HttpContext;

            var tenantId = httpContext?.GetRouteValue(TenantConstants.RouteKey)?.ToString()
                ?? NonEmpty(httpContext?.User.GetSchemaId())
                ?? FallbackValue;

            var userId = NonEmpty(httpContext?.User.GetUserId()) ?? FallbackValue;
            var clientIp = httpContext?.Connection.RemoteIpAddress?.ToString() ?? FallbackValue;
            var requestId = NonEmpty(httpContext?.TraceIdentifier) ?? FallbackValue;
            var method = httpContext is not null
                ? $"{httpContext.Request.Method} {httpContext.Request.Path}"
                : FallbackValue;

            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TenantId", tenantId));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserId", userId));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ClientIp", clientIp));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RequestId", requestId));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Method", method));
        }

        private static string? NonEmpty(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value;
    }
}
