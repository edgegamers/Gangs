using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;

namespace Leaderboard;

public class LeaderboardCommand(ILeaderboard leaderboard) : ICommand {
  public string Name => "css_lb";
  public string[] Aliases { get; } = ["css_lb", "css_leaderboard"];
  private (int, double)[]? cachedLeaderboard;

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    cachedLeaderboard ??= (await leaderboard.GetTopGangs()).ToArray();

    foreach (var (gangId, score) in cachedLeaderboard)
      info.ReplySync($"Gang {gangId} has a score of {score}");

    return CommandResult.SUCCESS;
  }

  [GameEventHandler]
  public HookResult OnRoundStart(EventRoundStart ev, GameEventInfo info) {
    cachedLeaderboard = null;
    return HookResult.Continue;
  }
}