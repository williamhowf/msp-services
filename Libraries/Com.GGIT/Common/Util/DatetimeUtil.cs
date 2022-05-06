using Com.GGIT.Enumeration;
using System;

namespace Com.GGIT.Common.Util
{
    public class DatetimeUtil
    {
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Convert epoch(seconds) to datetime UTC
        /// </summary>
        /// <param name="unixTime"></param>
        /// <returns></returns>
        public static DateTime FromUnixTime(long unixTime)
        {
            return epoch.AddSeconds(unixTime);
        }

        /// <summary>
        /// Convert epoch(seconds) to datetime UTC
        /// </summary>
        /// <param name="unixTime"></param>
        /// <returns></returns>
        public static DateTime FromUnixTimeSeconds(long unixTime)
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixTime).UtcDateTime;
        }

        /// <summary>
        /// Convert epoch(milliseconds) to datetime UTC
        /// </summary>
        /// <param name="unixTime"></param>
        /// <returns></returns>
        public static DateTime FromUnixTimeMilliseconds(long unixTime)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(unixTime).UtcDateTime;
        }

        /// <summary>
        /// Convert datetime(UTC) to Unix epoch(or Unix timestamp)[seconds]
        /// </summary>
        /// <param name="date">Datetime Utc format</param>
        /// <returns></returns>
        public static long ToUnixTimeSeconds(DateTime datetime)
        {
            return new DateTimeOffset(datetime, TimeSpan.Zero).ToUnixTimeSeconds();
        }

        /// <summary>
        /// Convert datetime(UTC) to Unix epoch(or Unix timestamp)[milliseconds]
        /// </summary>
        /// <param name="date">Datetime Utc format</param>
        /// <returns></returns>
        public static long ToUnixTimeMilliseconds(DateTime datetime)
        {
            return new DateTimeOffset(datetime, TimeSpan.Zero).ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// Get current UTC datetime
        /// </summary>
        /// <returns></returns>
        public static DateTime GetDatetimeUtc()
        {
            return DateTime.UtcNow;
        }

        /// <summary>
        /// Get current Unix epoch in seconds
        /// </summary>
        /// <returns></returns>
        public static long GetEpochSeconds()
        {
            return DateTimeOffset.Now.ToUnixTimeSeconds();
        }

        /// <summary>
        /// Get current epoch in milliseconds
        /// </summary>
        /// <returns></returns>
        public static long GetEpochMilliseconds()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// Return TRUE when datetime2 is older than datetime1
        /// </summary>
        /// <param name="Datetime1">Current datetime</param>
        /// <param name="Datetime2">Target/Expired datetime</param>
        /// <returns></returns>
        public static bool ExpiredDatetime(DateTime Datetime1, DateTime Datetime2)
        {
            return (Datetime1 - Datetime2).TotalSeconds > 0;
        }

        /// <summary>
        /// Return TRUE when UnixTime2 is older than UnixTime1. Input arguments MUST IN SECONDS.
        /// </summary>
        /// <param name="UnixTime1">Current epoch</param>
        /// <param name="UnixTime2">Target/Expired epoch</param>
        /// <returns></returns>
        public static bool ExpiredUnixTimeSeconds(long UnixTime1, long UnixTime2)
        {
            return (UnixTime1 - UnixTime2) > 0;
        }

        /// <summary>
        /// Return TRUE when UnixTime2 is older than UnixTime1. Input arguments MUST IN MILLISECONDS.
        /// </summary>
        /// <param name="UnixTime1">Current epoch</param>
        /// <param name="UnixTime2">Target/Expired epoch</param>
        /// <returns></returns>
        public static bool ExpiredUnixTimeMilliseconds(long UnixTime1, long UnixTime2)
        {
            return (UnixTime1 - UnixTime2) > 0;
        }

        /// <summary>
        /// Convert epoch milliseconds to seconds
        /// </summary>
        /// <param name="UnixTime"></param>
        /// <returns></returns>
        public static long ConvertUnixMillisecondsToSeconds(long UnixTime)
        {
            return Convert.ToInt64(UnixTime.ToString().Substring(0, 10));
        }

        /// <summary>
        /// Convert epoch seconds to milliseconds
        /// </summary>
        /// <param name="UnixTime"></param>
        /// <returns></returns>
        public static long ConvertUnixSecondsToMilliseconds(long UnixTime)
        {
            return UnixTime * 1000;
        }

        /// <summary>
        /// Return date string in ddMMyyyy format. i.e.: 23052019
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string FormatDayMonthYear(DateTime date)
        {
            return date.ToString(DatetimeFormatEnum.ddMMyyyy.ToValue());
        }

        /// <summary>
        /// Return date string in yyyyMMdd format. i.e.: 20190523
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string FormatYearMonthDay(DateTime date)
        {
            return date.ToString(DatetimeFormatEnum.yyyyMMdd.ToValue());
        }

        /// <summary>
        /// Return datetime string in yyyyMMddHHmm format. i.e.: 201905231158
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string FormatTimestamp(DateTime datetime)
        {
            return datetime.ToString(DatetimeFormatEnum.yyyyMMddHHmm.ToValue());
        }

        /// <summary>
        /// Return datetime string in yyyyMMddHHmmss format. i.e.: 20190523115803
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string FormatTimestampWithSeconds(DateTime datetime)
        {
            return datetime.ToString(DatetimeFormatEnum.yyyyMMddHHmmss.ToValue());
        }

        /// <summary>
        /// Return datetime string in yyyyMMddHHmmssfff format. i.e.: 20190523115803541
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string FormatTimestampWithMilliseconds(DateTime datetime)
        {
            return datetime.ToString(DatetimeFormatEnum.yyyyMMddHHmmssfff.ToValue());
        }

        /// <summary>
        /// Convert second to milliseconds
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public static int ConvertSecondToMilliseconds(int second)
        {
            return second * 1000;
        }

        /// <summary>
        /// Convert minute to milliseconds
        /// </summary>
        /// <param name="minute"></param>
        /// <returns></returns>
        public static int ConvertMinuteToMilliseconds(int minute)
        {
            return ConvertSecondToMilliseconds(minute * 60);
        }
    }
}
