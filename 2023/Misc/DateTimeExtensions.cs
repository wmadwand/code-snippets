using System;
using System.Globalization;
using UnityEngine;

public static class DateTimeExtensions
{
    static readonly DateTime kUnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    public static int ToUnixTimestamp(this DateTime dt)
    {
        /*TEMP*/dt = DateTime.Now;
        TimeSpan ts = dt - kUnixEpoch;
        return Convert.ToInt32(ts.TotalSeconds, CultureInfo.InvariantCulture);
    }

    public static DateTime ToDateTimeFromUnixTimestmap(this int timestamp)
    {
        var ts = new TimeSpan(TimeSpan.TicksPerSecond * timestamp);
        return kUnixEpoch + ts;
    }

    public static string ConvertToRemainTimeString(this int seconds)
    {
        if (seconds > 3600)
        {
            var hours = seconds / 3600;
            seconds -= hours * 3600;
            var minutes = seconds / 60;
            seconds -= minutes * 60;
            return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        }

        if (seconds > 60)
        {
            return $"{seconds / 60 :D2}:{seconds % 60 :D2}";
        }

        return $"{seconds}s";
    }

}
