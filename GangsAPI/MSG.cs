using GangsAPI.Data.Command;
using Microsoft.Extensions.Localization;

namespace GangsAPI;

public enum MSG {
  NOT_IN_GANG,
  PREFIX,
  GENERIC_PLAYER_NOT_FOUND,
  SOONTM,
  GENERIC_PLAYER_ONLY,
  GENERIC_NOPERM,
  GENERIC_NOPERM_NODE,
  GENERIC_NOPERM_RANK
}

public static class LocaleExtensions {
  public static string Key(this MSG msg) {
    return msg switch {
      MSG.NOT_IN_GANG => "command.gang.not_in_gang",
      MSG.PREFIX => "prefix",
      MSG.GENERIC_PLAYER_NOT_FOUND => "generic.player.not_found",
      MSG.SOONTM => "generic.soontm",
      MSG.GENERIC_PLAYER_ONLY => "generic.player.only",
      MSG.GENERIC_NOPERM => "generic.no_permission",
      MSG.GENERIC_NOPERM_NODE => "generic.no_permission.node",
      MSG.GENERIC_NOPERM_RANK => "generic.no_permission.rank",
      _ => throw new ArgumentOutOfRangeException(nameof(msg), msg, null)
    };
  }

  public static string Get(this IStringLocalizer localizer, MSG msg,
    params object[] args) {
    return localizer[msg.Key(), args].Value;
  }
}