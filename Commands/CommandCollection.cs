using Microsoft.Extensions.DependencyInjection;
using GangsAPI.Extensions;

namespace Commands;

public static class CommandCollection {
  public static void RegisterCommands(this IServiceCollection provider) {
    provider.AddPluginBehavior<GangCommand>();
  }
}