using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Perks;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using Microsoft.Extensions.DependencyInjection;

namespace Leaderboard;

public class LeaderboardCommand(ILeaderboard leaderboard,
  IServiceProvider provider) : ICommand {
  private (int, double)[]? cachedLeaderboard;
  public string Name => "css_gangrank";
  public string[] Aliases => ["css_gangranks", "css_gangtop", "css_ganglb"];

  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    cachedLeaderboard ??= (await leaderboard.GetTopGangs()).ToArray();

    var desc = provider.GetService<IMotdPerk>();

    var index = 0;
    foreach (var (gangId, score) in cachedLeaderboard) {
      var gang = await gangs.GetGang(gangId);
      if (gang == null) continue;
      var motd               = "";
      if (desc != null) motd = await desc.GetMotd(gang) ?? string.Empty;

      info.ReplySync(
        $"{ChatColors.Orange}{++index}. {ChatColors.Red}{gang.Name}{ChatColors.Grey}: {ChatColors.White}{score} {ChatColors.Grey}{motd}");
    }

    return CommandResult.SUCCESS;
  }

  [GameEventHandler]
  public HookResult OnRoundStart(EventRoundStart ev, GameEventInfo info) {
    cachedLeaderboard = null;
    return HookResult.Continue;
  }
}