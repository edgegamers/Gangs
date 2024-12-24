using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;

namespace Leaderboard;

public class LeaderboardCommand(ILeaderboard leaderboard, IGangManager gangs)
  : ICommand {
  private (int, double)[]? cachedLeaderboard;
  public string Name => "css_gangrank";
  public string[] Aliases => ["css_gangranks", "css_gangtop", "css_ganglb"];

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    cachedLeaderboard ??= (await leaderboard.GetTopGangs()).ToArray();

    var index = 0;
    foreach (var (gangId, score) in cachedLeaderboard) {
      var gang = await gangs.GetGang(gangId);
      if (gang == null) continue;
      info.ReplySync(
        $"{ChatColors.Orange}{++index}. {ChatColors.Red}{gang.Name}{ChatColors.Grey}: {ChatColors.White}{score}");
    }

    return CommandResult.SUCCESS;
  }

  [GameEventHandler]
  public HookResult OnRoundStart(EventRoundStart ev, GameEventInfo info) {
    cachedLeaderboard = null;
    return HookResult.Continue;
  }
}