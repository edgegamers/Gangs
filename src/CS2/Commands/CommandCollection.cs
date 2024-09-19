using GangsAPI.Extensions;
using GangsAPI.Services.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Commands;

public static class CommandCollection {
  public static void RegisterCommands(this IServiceCollection provider) {
    provider.AddPluginBehavior<ICommandManager, CommandManager>();
  }
}