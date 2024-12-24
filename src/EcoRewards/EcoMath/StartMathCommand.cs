using CounterStrikeSharp.API;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace EcoRewards.EcoMath;

public class StartMathCommand(IServiceProvider provider) : ICommand {
  private readonly IMathService? math = provider.GetService<IMathService>();
  public string Name => "css_startmath";

  public string[] RequiredFlags => ["@css/root"];

  public Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (math == null) return Task.FromResult(CommandResult.ERROR);
    if (executor == null) return Task.FromResult(CommandResult.PLAYER_ONLY);

    Server.NextFrame(() => math.StartMath());
    return Task.FromResult(CommandResult.SUCCESS);
  }
}