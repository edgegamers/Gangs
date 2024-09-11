using GangsAPI.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace StatsTracker;

public static class StatsTrackerCollection {
  public static void RegisterStatsTracker(this IServiceCollection provider) {
    provider.AddPluginBehavior<RoundStatsTracker>();
  }
}