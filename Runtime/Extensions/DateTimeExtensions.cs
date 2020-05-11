using System;

public static class DateTimeExtensions
{
    public static long ToUnixMilliseconds(DateTime time)
    {
        return (long)((time - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
    }

    public static DateTime FromUnixMilliseconds(long milliseconds)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(milliseconds);
    }
}

public static class TimeSpanExtensions
{
    public static string ToReadableAgeString(this TimeSpan span)
    {
        return $"{span.Days / 365.25:0}";
    }

    public static string ToReadableString(this TimeSpan span, bool showSeconds = true, bool showMinutes = true, bool showHours = true, bool showDays = true)
    {
        string formatted = string.Format("{0}{1}{2}{3}",
            span.Duration().Days > 0 && showDays ? string.Format("{0:0} day{1}, ", span.Days, span.Days == 1 ? string.Empty : "s") : string.Empty,
            span.Duration().Hours > 0 && showHours ? string.Format("{0:0} hour{1}, ", span.Hours, span.Hours == 1 ? string.Empty : "s") : string.Empty,
            span.Duration().Minutes > 0 && showMinutes ? string.Format("{0:0} minute{1}, ", span.Minutes, span.Minutes == 1 ? string.Empty : "s") : string.Empty,
            span.Duration().Seconds > 0 && showSeconds ? string.Format("{0:0} second{1}", span.Seconds, span.Seconds == 1 ? string.Empty : "s") : string.Empty);

        if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

        if (string.IsNullOrEmpty(formatted)) formatted = "0 seconds";

        return formatted;
    }


}