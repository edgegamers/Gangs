using CounterStrikeSharp.API.Core;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Raffle;

public class RaffleCommand(IServiceProvider provider) : ICommand {
  private readonly IEcoManager eco = provider.GetRequiredService<IEcoManager>();

  private readonly IRaffleManager raffle =
    provider.GetRequiredService<IRaffleManager>();

  private readonly IStringLocalizer locale =
    provider.GetRequiredService<IStringLocalizer>();

  public string Name => "css_raffle";

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;
    if (info.ArgCount != 1) return CommandResult.PRINT_USAGE;

    if (raffle.Raffle == null) {
      info.ReplySync(locale.Get(MSG.RAFFLE_NOTACTIVE));
      return CommandResult.SUCCESS;
    }

    if (!raffle.AreEntriesOpen()) {
      info.ReplySync(locale.Get(MSG.RAFFLE_NOTOPEN));
      return CommandResult.SUCCESS;
    }

    if (await eco.TryPurchase(executor, raffle.Raffle.BuyIn, true,
      "Raffle Ticket", true) < 0)
      return CommandResult.SUCCESS;

    raffle.Raffle.AddPlayer(executor.Steam);
    return CommandResult.SUCCESS;
  }
}