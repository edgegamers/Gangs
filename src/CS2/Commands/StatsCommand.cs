using System.Collections;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Data.Stat;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;

namespace Commands;

public class StatsCommand(IServiceProvider provider) : ICommand {
  public string Name => "css_stats";

  private readonly IPlayerStatManager playerStats =
    provider.GetRequiredService<IPlayerStatManager>();

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;
    var stats      = provider.GetRequiredService<IStatManager>();
    var registered = await stats.GetStats() as IEnumerable<IStat<object>>;
    if (registered is null) return CommandResult.ERROR;
    foreach (var stat in registered) {
      var (success, value) =
        await playerStats.GetForPlayer<object>(executor, stat.StatId);
      if (!success) continue;
      if (value is null) continue;
      info.ReplySync($"{stat.Name}{value}");
    }

    return CommandResult.SUCCESS;
  }
}