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

  public Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    var amo = 100;
    if (info.ArgCount == 2)
      if (!int.TryParse(info.Args[1], out amo))
        return Task.FromResult(CommandResult.PRINT_USAGE);

    raffle.StartRaffle(amo);
    return Task.FromResult(CommandResult.SUCCESS);
  }
}