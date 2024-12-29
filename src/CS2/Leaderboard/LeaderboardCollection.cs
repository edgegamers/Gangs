using GangsAPI.Extensions;
using GangsAPI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Leaderboard;

public static class LeaderboardCollection {
  public static void RegisterLeaderboard(this IServiceCollection provider) {
    provider.AddPluginBehavior<ILeaderboard, MSLeaderboard>();
    provider.AddPluginBehavior<EloAssigner>();
    provider.AddPluginBehavior<CreditAssigner>();
  }
}