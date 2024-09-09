using System.Globalization;

namespace GangsAPI.Extensions;

public static class StringExtensions {
  /// <summary>
  /// Converts a given string to Upper Title Case.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  public static string ToTitleCase(this string str) {
    return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
  }
}