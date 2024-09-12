using System.Reflection;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Data.Stat;
using GangsAPI.Extensions;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;

namespace Commands;

public class StatsCommand(IServiceProvider provider) : ICommand {
  private readonly IPlayerStatManager playerStats =
    provider.GetRequiredService<IPlayerStatManager>();

  public string Name => "css_stats";

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;
    var stats = provider.GetRequiredService<IStatManager>().Stats;
    foreach (var stat in stats) {
      info.ReplySync($"Getting stat... {stat.Name}");
      var getForPlayer = playerStats.GetType()
       .GetMethod("GetForPlayer")
      ?.MakeGenericMethod(stat.ValueType);
      if (getForPlayer == null) {
        info.ReplySync("Failed to get method for stat.");
        continue;
      }

      info.ReplySync($"Invoking method for stat... {stat.Name}");

      dynamic? result =
        await getForPlayer.InvokeAsync(playerStats, executor, stat.StatId);
      bool   success = result.Item1;
      object value   = result.Item2;

      if (!success) {
        info.ReplySync($"Failed to get stat {stat.Name}");
        continue;
      }

      if (value is null) {
        info.ReplySync($"Stat {stat.Name} is null.");
        continue;
      }

      info.ReplySync($"{stat.Name}{value}");
    }

    return CommandResult.SUCCESS;
  }
}