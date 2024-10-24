using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Raffle;

public class StartRaffleCommand(IServiceProvider provider) : ICommand {
  private readonly IRaffleManager raffle =
    provider.GetRequiredService<IRaffleManager>();

  public string Name => "css_startraffle";
  public string[] RequiredFlags => ["@css/root"];
  public string[] Usage => ["", "<amount>"];

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    var amo = 100;
    if (info.ArgCount == 2)
      if (!int.TryParse(info.Args[1], out amo))
        return CommandResult.PRINT_USAGE;

    await Server.NextFrameAsync(() => { raffle.StartRaffle(amo); });
    return CommandResult.SUCCESS;
  }
}