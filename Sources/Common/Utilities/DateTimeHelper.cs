namespace ClubbyBook.Common.Utilities
{
    using System;

    public static class DateTimeHelper
    {
        private const string ukrainianTimeZoneId = "FLE Standard Time";
        private static TimeZoneInfo ukrainianTimeZoneCache = null;

        public static DateTime Now
        {
            get
            {
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, DateTimeHelper.UkrainianTimeZone);
            }
        }

        private static TimeZoneInfo UkrainianTimeZone
        {
            get
            {
                if (ukrainianTimeZoneCache == null)
                    ukrainianTimeZoneCache = TimeZoneInfo.FindSystemTimeZoneById(DateTimeHelper.ukrainianTimeZoneId);
                return ukrainianTimeZoneCache;
            }
        }
    }
}