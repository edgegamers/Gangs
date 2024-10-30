using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;

namespace Leaderboard;

public class LeaderboardCommand(ILeaderboard leaderboard, IGangManager gangs)
  : ICommand {
  public string Name => "css_gangrank";
  public string[] Aliases => ["css_gangranks", "css_gangtop", "css_ganglb"];
  private (int, double)[]? cachedLeaderboard;

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    cachedLeaderboard ??= (await leaderboard.GetTopGangs()).ToArray();

    var index = 0;
    foreach (var (gangId, score) in cachedLeaderboard) {
      var gang = await gangs.GetGang(gangId);
      if (gang == null) continue;
      info.ReplySync($"{++index} {gang.Name}: {score}");
    }

    return CommandResult.SUCCESS;
  }

  [GameEventHandler]
  public HookResult OnRoundStart(EventRoundStart ev, GameEventInfo info) {
    cachedLeaderboard = null;
    return HookResult.Continue;
  }
}