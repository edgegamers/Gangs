using CounterStrikeSharp.API.Core;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Raffle;

public class RaffleCommand(IServiceProvider provider) : ICommand {
  private readonly IEcoManager eco = provider.GetRequiredService<IEcoManager>();

  private readonly IRaffleManager raffle =
    provider.GetRequiredService<IRaffleManager>();

  private readonly ICommandManager commands =
    provider.GetRequiredService<ICommandManager>();

  public string Name => "css_raffle";

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;
    if (info.ArgCount != 1) return CommandResult.PRINT_USAGE;

    if (raffle.Raffle == null)
      // no raffle is currently running
      return CommandResult.SUCCESS;

    if (!raffle.AreEntriesOpen())
      // entries are closed
      return CommandResult.SUCCESS;

    if (await eco.TryPurchase(executor, raffle.Raffle.BuyIn, true,
      "Raffle Ticket", true) < 0)
      return CommandResult.SUCCESS;

    raffle.Raffle.AddPlayer(executor.Steam);
    return CommandResult.SUCCESS;
  }
}