namespace Worktime.Global
{
    public static class TimeConvert
    {
        public static DateTime ConvertToGMT11(DateTime dt)
        {
            // Set the timezone to New Caledonia (GMT+11)
            TimeZoneInfo newCaledoniaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific/Noumea");

            // Convert the local time to New Caledonia time (GMT+11)
            DateTime newCaledoniaTime = TimeZoneInfo.ConvertTime(dt, newCaledoniaTimeZone);

            return newCaledoniaTime;
        }
    }
}
