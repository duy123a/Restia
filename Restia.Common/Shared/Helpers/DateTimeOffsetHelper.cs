using Microsoft.Extensions.Configuration;

namespace Restia.Common.Shared.Helpers
{
    public static class DateTimeOffsetHelper
    {
        private const string DefaultTimeZoneId = "Asia/Ho_Chi_Minh";

        private static TimeZoneInfo _appTimeZone =
            ResolveTimeZone(DefaultTimeZoneId);

        public static TimeZoneInfo AppTimeZone => _appTimeZone;

        public static void Initialize(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            _appTimeZone = ResolveTimeZone(configuration["TimeZone"]);
        }

        public static DateTimeOffset ToAppTime(
            this DateTimeOffset dateTimeOffset)
        {
            return TimeZoneInfo.ConvertTime(dateTimeOffset, _appTimeZone);
        }

        public static DateTimeOffset? ToAppTime(
            this DateTimeOffset? dateTimeOffset)
        {
            return dateTimeOffset.HasValue
                ? dateTimeOffset.Value.ToAppTime()
                : null;
        }

        public static DateTimeOffset FromAppTime(DateTime localDateTime)
        {
            localDateTime = DateTime.SpecifyKind(
                localDateTime,
                DateTimeKind.Unspecified);

            if (_appTimeZone.IsInvalidTime(localDateTime)
                || _appTimeZone.IsAmbiguousTime(localDateTime))
            {
                throw new ArgumentException(
                    "The specified local time does not exist in the app timezone.",
                    nameof(localDateTime));
            }

            var offset = _appTimeZone.GetUtcOffset(localDateTime);

            return new DateTimeOffset(localDateTime, offset);
        }

        private static TimeZoneInfo ResolveTimeZone(string? timeZoneId)
        {
            timeZoneId = string.IsNullOrWhiteSpace(timeZoneId)
                ? DefaultTimeZoneId
                : timeZoneId.Trim();

            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
                if (timeZoneId.Equals(
                    DefaultTimeZoneId,
                    StringComparison.OrdinalIgnoreCase))
                {
                    throw;
                }

                return TimeZoneInfo.FindSystemTimeZoneById(
                    DefaultTimeZoneId);
            }
        }
    }
}