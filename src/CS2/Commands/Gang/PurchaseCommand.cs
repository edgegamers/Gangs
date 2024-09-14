using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Perks;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Commands.Gang;

public class PurchaseCommand(IServiceProvider provider) : ICommand {
  public string Name => "purchase";

  public string[] Usage => ["<item>"];

  private readonly IStringLocalizer localizer =
    provider.GetRequiredService<IStringLocalizer>();

  private readonly IPerkManager perks =
    provider.GetRequiredService<IPerkManager>();

  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly IEcoManager eco = provider.GetRequiredService<IEcoManager>();

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;
    if (info.ArgCount != 2) return CommandResult.PRINT_USAGE;

    var perk = getPerk(info[1]);
    if (perk == null) {
      info.ReplySync(localizer.Get(MSG.PERK_NOT_FOUND, info[1]));
      return CommandResult.SUCCESS;
    }

    var gangPlayer = await players.GetPlayer(executor.Steam);
    if (gangPlayer == null) {
      info.ReplySync(localizer.Get(MSG.GENERIC_ERROR_INFO,
        "Could not fetch gang player"));
      return CommandResult.SUCCESS;
    }

    var cost = await perk.GetCost(gangPlayer);
    if (cost == null) {
      localizer.Get(MSG.PERK_UNPURCHASABLE_WITH_ITEM, perk.Name);
      return CommandResult.SUCCESS;
    }

    var remaining =
      await eco.TryPurchase(executor, cost.Value, true, perk.Name);
    if (remaining < 0) return CommandResult.SUCCESS;

    await perk.OnPurchase(gangPlayer);
    return CommandResult.SUCCESS;
  }

  private IPerk? getPerk(string id) {
    return perks.Perks.FirstOrDefault(p => p.StatId == id);
  }
}