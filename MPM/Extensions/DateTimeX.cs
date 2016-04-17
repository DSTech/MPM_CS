namespace System {
    public static class DateTimeX {
        public static DateTime UnixEpoc => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static Int64 ToUnixTimeStamp(this DateTime dateTime) => Convert.ToInt64(dateTime.ToUniversalTime().Subtract(UnixEpoc).TotalSeconds);

        public static Double ToUnixTimeStampDouble(this DateTime dateTime) => dateTime.ToUniversalTime().Subtract(UnixEpoc).TotalSeconds;

        public static DateTime FromUnixTimeStamp(this Int64 timeStamp) => UnixEpoc.AddSeconds(timeStamp);

        public static DateTime FromUnixTimeStamp(this Double timeStamp) => UnixEpoc.AddSeconds(timeStamp);
    }
}
