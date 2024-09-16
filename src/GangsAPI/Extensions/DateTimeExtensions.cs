namespace GangsAPI.Extensions;

public static class DateTimeExtensions {
  public static string FormatRelative(this DateTime dt) {
    var ts    = new TimeSpan(DateTime.UtcNow.Ticks - dt.Ticks);
    var delta = Math.Abs(ts.TotalSeconds);

    switch (delta) {
      case < 1 * 60:
        return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
      case < 2 * 60:
        return "a minute ago";
      case < 45 * 60:
        return ts.Minutes + " minutes ago";
      case < 90 * 60:
        return "an hour ago";
      case < 24 * 60 * 60:
        return ts.Hours + " hours ago";
      case < 48 * 60 * 60:
        return "yesterday";
      // If < 3 days, return the number of days + hours
      case < 3 * 24 * 60 * 60: {
        var days  = ts.Days;
        var hours = ts.Hours;

        if (hours == 0) return days == 1 ? "one day ago" : $"{days} days ago";
        return days == 1 ? "one day ago" : $"{days} days, {hours} hours ago";
      }
      case < 30 * 24 * 60 * 60:
        return ts.Days + " days ago";
      case < 12 * 30 * 24 * 60 * 60: {
        var months = (int)Math.Floor((double)ts.Days / 30);
        return months <= 1 ? "one month ago" : months + " months ago";
      }
      default: {
        var years = (int)Math.Floor((double)ts.Days / 365);
        return years <= 1 ? "one year ago" : years + " years ago";
      }
    }
  }
}