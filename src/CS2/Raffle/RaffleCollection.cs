using GangsAPI.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Raffle;

public static class RaffleCollection {
  public static void RegisterRaffle(this IServiceCollection provider) {
    provider.AddPluginBehavior<IRaffleManager, RaffleManager>();
    provider.AddPluginBehavior<RaffleCommand>();
    provider.AddPluginBehavior<StartRaffleCommand>();
  }
}