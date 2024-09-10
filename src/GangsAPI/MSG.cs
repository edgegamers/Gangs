using Microsoft.Extensions.Localization;

namespace GangsAPI;

public enum MSG {
  COMMAND_GANG_NOTINGANG,
  COMMAND_GANG_CREATE_ALREADY_EXISTS,
  COMMAND_INVITE_MAXINVITES,
  COMMAND_INVITE_ALREADY_INVITED,
  COMMAND_INVITE_ALREADY_IN_GANG,
  COMMAND_INVITE_IN_YOUR_GANG,
  COMMAND_INVITE_SUCCESS,
  GENERIC_STEAM_NOT_FOUND,
  COMMAND_BALANCE_NONE,
  COMMAND_BALANCE,
  COMMAND_BALANCE_PLURAL,
  COMMAND_BALANCE_OTHER_NONE,
  COMMAND_BALANCE_OTHER,
  COMMAND_BALANCE_OTHER_PLURAL,
  COMMAND_BALANCE_SET,
  COMMAND_USAGE,
  COMMAND_INVALID_PARAM,
  PREFIX,
  GENERIC_PLAYER_NOT_FOUND,
  GENERIC_PLAYER_FOUND_MULTIPLE,
  SOONTM,
  GENERIC_PLAYER_ONLY,
  GENERIC_NOPERM,
  GENERIC_NOPERM_NODE,
  GENERIC_NOPERM_RANK,
  GENERIC_ERROR,
  GENERIC_ERROR_INFO,
  PLAYER_CURRENCY,
  PLAYER_CURRENCY_PLURAL,
  GANG_CURRENCY,
  GANG_CURRENCY_PLURAL,
  COLOR_DEFAULT,
  COLOR_EMPHASIS,
  COLOR_NUMBERL,
  COLOR_SPECIAL,
  COLOR_COMMAND,
  COLOR_CURRENCY,
  COLOR_TARGET,
  ALREADY_IN_GANG,
  NOT_IN_GANG
}

public static class LocaleExtensions {
  public static string Key(this MSG msg) {
    return msg switch {
      MSG.COMMAND_GANG_NOTINGANG => "command.gang.not_in_gang",
      MSG.COMMAND_GANG_CREATE_ALREADY_EXISTS =>
        "command.gang.create.already_exists",
      MSG.COMMAND_INVITE_MAXINVITES => "command.gang.invite.max_invites",
      MSG.COMMAND_INVITE_ALREADY_INVITED =>
        "command.gang.invite.already_invited",
      MSG.COMMAND_INVITE_ALREADY_IN_GANG =>
        "command.gang.invite.already_ingang",
      MSG.COMMAND_INVITE_IN_YOUR_GANG =>
        "command.gang.invite.already_inyourgang",
      MSG.COMMAND_INVITE_SUCCESS        => "command.gang.invite.success",
      MSG.COMMAND_BALANCE_NONE          => "command.balance.none",
      MSG.COMMAND_BALANCE               => "command.balance",
      MSG.COMMAND_BALANCE_OTHER         => "command.balance.other",
      MSG.COMMAND_BALANCE_OTHER_PLURAL  => "command.balance.other.plural",
      MSG.COMMAND_BALANCE_OTHER_NONE    => "command.balance.other.none",
      MSG.COMMAND_BALANCE_PLURAL        => "command.balance.plural",
      MSG.COMMAND_BALANCE_SET           => "command.balance.set",
      MSG.COMMAND_USAGE                 => "command.usage",
      MSG.COMMAND_INVALID_PARAM         => "command.invalid_parameter",
      MSG.PREFIX                        => "prefix",
      MSG.GENERIC_PLAYER_NOT_FOUND      => "generic.player.not_found",
      MSG.SOONTM                        => "generic.soontm",
      MSG.GENERIC_PLAYER_ONLY           => "generic.player.only",
      MSG.GENERIC_STEAM_NOT_FOUND       => "generic.player.steam_not_found",
      MSG.GENERIC_NOPERM                => "generic.no_permission",
      MSG.GENERIC_NOPERM_NODE           => "generic.no_permission.node",
      MSG.GENERIC_NOPERM_RANK           => "generic.no_permission.rank",
      MSG.GENERIC_PLAYER_FOUND_MULTIPLE => "generic.player.found_multiple",
      MSG.GENERIC_ERROR                 => "generic.error",
      MSG.GENERIC_ERROR_INFO            => "generic.error.info",
      MSG.PLAYER_CURRENCY               => "currency.player",
      MSG.PLAYER_CURRENCY_PLURAL        => "currency.player.plural",
      MSG.GANG_CURRENCY                 => "currency.gang",
      MSG.GANG_CURRENCY_PLURAL          => "currency.gang.plural",
      MSG.COLOR_DEFAULT                 => "color.default",
      MSG.COLOR_EMPHASIS                => "color.emph",
      MSG.COLOR_NUMBERL                 => "color.number",
      MSG.COLOR_SPECIAL                 => "color.special",
      MSG.COLOR_COMMAND                 => "color.command",
      MSG.COLOR_CURRENCY                => "color.currency",
      MSG.COLOR_TARGET                  => "color.target",
      MSG.ALREADY_IN_GANG               => "gang.already_in_gang",
      MSG.NOT_IN_GANG                   => "gang.not_in_gang",

      _ => throw new ArgumentOutOfRangeException(nameof(msg), msg, null)
    };
  }

  public static string Get(this IStringLocalizer localizer, MSG msg,
    params object[] args) {
    try { return localizer[msg.Key(), args].Value; } catch (FormatException e) {
      throw new FormatException(
        $"There was an error formatting {msg.Key()} ({localizer[msg.Key()]})",
        e);
    }
  }
}