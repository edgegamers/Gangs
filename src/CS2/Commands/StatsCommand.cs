using System.Reflection;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Data.Stat;
using GangsAPI.Extensions;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Serilog.Core;

namespace Commands;

public class StatsCommand(IServiceProvider provider) : ICommand {
  private readonly IPlayerStatManager playerStats =
    provider.GetRequiredService<IPlayerStatManager>();

  private readonly IStringLocalizer localizer =
    provider.GetRequiredService<IStringLocalizer>();

  public string Name => "css_stats";

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;
    var stats        = provider.GetRequiredService<IStatManager>().Stats;
    var getForPlayer = playerStats.GetType().GetMethod("GetForPlayer");

    if (getForPlayer == null) {
      info.ReplySync(localizer.Get(MSG.GENERIC_ERROR_INFO,
        "Could not find GetForPlayer"));
      return CommandResult.ERROR;
    }

    foreach (var stat in stats) {
      var getForPlayerTyped = getForPlayer.MakeGenericMethod(stat.ValueType);
      dynamic? task =
        getForPlayerTyped.Invoke(playerStats, [executor.Steam, stat.StatId]);

      if (task == null) {
        info.ReplySync(localizer.Get(MSG.GENERIC_ERROR_INFO,
          $"Could not get stat {stat.StatId}"));
        continue;
      }

      var    result  = await task;
      bool   success = result.Item1;
      object value   = result.Item2;

      if (!success || value is null) continue;
      var str = (value.ToString() ?? $"{stat.Name} {stat.Description}");
      foreach (var s in str.Split("\n")) info.ReplySync(s);
    }

    return CommandResult.SUCCESS;
  }
}