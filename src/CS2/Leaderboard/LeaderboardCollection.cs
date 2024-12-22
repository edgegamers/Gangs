using GangsAPI.Extensions;
using GangsAPI.Services;
using GangsAPI.Services.Server;
using Microsoft.Extensions.DependencyInjection;

namespace Leaderboard;

public static class LeaderboardCollection {
  public static void RegisterLeaderboard(this IServiceCollection provider) {
    provider.AddPluginBehavior<ILeaderboard, MSLeaderboard>();
  }
}