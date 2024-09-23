using GangsAPI.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace EcoRewards;

public static class RewardsCollection {
  public const int MIN_PLAYERS  = 5;

  public static void RegisterRewards(this IServiceCollection provider) {
    provider.AddPluginBehavior<RoundWinListener>();
    provider.AddPluginBehavior<PeriodicRewarder>();
  }
}