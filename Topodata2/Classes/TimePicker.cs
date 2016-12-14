using System;

namespace Topodata2.Classes
{
    public static class TimePicker
    {
        public static DateTime GetLocalDateTime()
        {
            return GetLocalDateTime("Central Brazilian Standard Time");
        }

        private static DateTime GetLocalDateTime(string timeZoneId)
        {
            var timezone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var result = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);
            return result;
        }

    }
}