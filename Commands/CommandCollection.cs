using Microsoft.Extensions.DependencyInjection;
using GangsAPI.Extensions;
using GangsAPI.Services;
using GangsAPI.Services.Commands;

namespace Commands;

public static class CommandCollection {
  public static void RegisterCommands(this IServiceCollection provider) {
    provider.AddPluginBehavior<ICommandManager, CommandManager>();
  }
}