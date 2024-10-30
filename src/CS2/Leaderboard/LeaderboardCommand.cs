using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;

namespace Leaderboard;

public class LeaderboardCommand(ILeaderboard leaderboard) : ICommand {
  public string Name => "css_gangrank";
  public string[] Aliases => ["css_gangranks", "css_gangtop", "css_ganglb"];
  private (int, double)[]? cachedLeaderboard;

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    cachedLeaderboard ??= (await leaderboard.GetTopGangs()).ToArray();

    foreach (var (gangId, score) in cachedLeaderboard)
      info.ReplySync($"Gang {gangId} has a score of {score}");

    info.ReplySync($"{cachedLeaderboard.Length} total ranked gangs.");
    return CommandResult.SUCCESS;
  }

  [GameEventHandler]
  public HookResult OnRoundStart(EventRoundStart ev, GameEventInfo info) {
    cachedLeaderboard = null;
    return HookResult.Continue;
  }
}