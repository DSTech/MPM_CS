using System;

namespace MPM.Extensions {

	public static class DateTimeExtensions {
		public static DateTime UnixEpoc => new DateTime(1970, 1, 1);

		public static Int64 ToUnixTimeStamp(this DateTime dateTime) => Convert.ToInt64(dateTime.Subtract(UnixEpoc).TotalSeconds);

		public static Double ToUnixTimeStampDouble(this DateTime dateTime) => dateTime.Subtract(UnixEpoc).TotalSeconds;

		public static DateTime FromUnixTimeStamp(this Int64 timeStamp) => UnixEpoc.AddSeconds(timeStamp);

		public static DateTime FromUnixTimeStamp(this Double timeStamp) => UnixEpoc.AddSeconds(timeStamp);
	}
}
