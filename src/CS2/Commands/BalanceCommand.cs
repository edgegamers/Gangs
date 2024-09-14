using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Player;
using GangsAPI.Services.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Stats.Stat;

namespace Commands;

public class BalanceCommand(IServiceProvider provider) : ICommand {
  private readonly string id = new BalanceStat().StatId;

  private readonly IStringLocalizer localizer =
    provider.GetRequiredService<IStringLocalizer>();

  private readonly IPlayerStatManager playerStats =
    provider.GetRequiredService<IPlayerStatManager>();

  private readonly ITargeter targeter =
    provider.GetRequiredService<ITargeter>();

  public string Name => "css_balance";

  public string[] Usage => ["", "<player>", "<player> <amount>"];
  public string[] Aliases => ["css_balance", "css_credit", "css_credits"];

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;

    if (info.ArgCount == 1 || !executor.HasFlags("@css/ban")) {
      var (success, balance) =
        await playerStats.GetForPlayer<int>(executor.Steam, id);

      if (!success) {
        info.ReplySync(localizer.Get(MSG.COMMAND_BALANCE_NONE));
        return CommandResult.SUCCESS;
      }

      info.ReplySync(localizer.Get(MSG.COMMAND_BALANCE, balance));
      return CommandResult.SUCCESS;
    }

    if (info.ArgCount > 3) return CommandResult.PRINT_USAGE;

    var subject =
      await targeter.GetSingleTarget(info[1], out _, executor, localizer);

    if (subject == null) return CommandResult.SUCCESS;

    if (info.ArgCount == 2 || !executor.HasFlags("@css/root")) {
      var (success, balance) =
        await playerStats.GetForPlayer<int>(subject.Steam, id);

      if (!success) {
        info.ReplySync(localizer.Get(MSG.COMMAND_BALANCE_OTHER_NONE,
          subject.Name ?? subject.Steam.ToString()));
        return CommandResult.SUCCESS;
      }

      info.ReplySync(localizer.Get(MSG.COMMAND_BALANCE_OTHER,
        subject.Name ?? subject.Steam.ToString(), balance));
      return CommandResult.SUCCESS;
    }

    if (!int.TryParse(info[2], out var amount)) {
      info.ReplySync(localizer.Get(MSG.COMMAND_INVALID_PARAM, info[2],
        "an integer"));
      return CommandResult.INVALID_ARGS;
    }

    var pass = await playerStats.SetForPlayer(subject.Steam, id, amount);
    if (!pass) {
      info.ReplySync(localizer.Get(MSG.GENERIC_ERROR));
      return CommandResult.ERROR;
    }

    info.ReplySync(localizer.Get(MSG.COMMAND_BALANCE_SET,
      subject.Name ?? subject.Steam.ToString(), amount));
    return CommandResult.SUCCESS;
  }
}