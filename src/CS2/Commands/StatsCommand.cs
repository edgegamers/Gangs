using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Menu;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Commands;

public class StatsCommand(IServiceProvider provider) : ICommand {
  private readonly IStringLocalizer localizer =
    provider.GetRequiredService<IStringLocalizer>();

  private readonly IPlayerStatManager playerStats =
    provider.GetRequiredService<IPlayerStatManager>();

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

    var list = new List<string>();

    foreach (var stat in stats) {
      var getForPlayerTyped = getForPlayer.MakeGenericMethod(stat.ValueType);
      dynamic? task =
        getForPlayerTyped.Invoke(playerStats, [executor.Steam, stat.StatId]);

      if (task == null) {
        info.ReplySync(localizer.Get(MSG.GENERIC_ERROR_INFO,
          $"Could not get stat {stat.StatId}"));
        continue;
      }

      var result = await task;
      if (result is null) continue;
      var str = result.ToString() ?? $"{stat.Name} {stat.Description}";
      list.Add(str);
    }

    var menu = new SimplePagedMenu(provider, list);
    await provider.GetRequiredService<IMenuManager>().OpenMenu(executor, menu);
    return CommandResult.SUCCESS;
  }
}