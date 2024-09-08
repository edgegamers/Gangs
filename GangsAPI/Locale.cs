using Microsoft.Extensions.Localization;

namespace GangsAPI;

public enum Locale {
  NOT_IN_GANG, PREFIX, PLAYER_NOT_FOUND
}

public static class LocaleExtensions {
  public static string Key(this Locale locale) {
    return locale switch {
      Locale.NOT_IN_GANG => "command.gang.not_in_gang",
      Locale.PREFIX => "prefix",
      Locale.PLAYER_NOT_FOUND => "generic.player.not_found",
      _ => throw new ArgumentOutOfRangeException(nameof(locale), locale, null)
    };
  }

  public static string
    Localize(this IStringLocalizer localizer, Locale locale) {
    return localizer[locale.Key()].Value;
  }

  public static string Localize(this IStringLocalizer localizer, Locale locale,
    params object[] args) {
    return localizer[locale.Key(), args].Value;
  }
}