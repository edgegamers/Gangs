using Commands;
using CounterStrikeSharp.API.Core;
using GangsAPI;
using GangsAPI.Extensions;
using GangsAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using Mock;

namespace GangsImpl;

public class GangServiceCollection : IPluginServiceCollection<IGangPlugin> {
  public void ConfigureServices(IServiceCollection serviceCollection) {
    serviceCollection.AddPluginBehavior<IGangManager, MockGangManager>();
    serviceCollection.AddPluginBehavior<IPlayerManager, MockPlayerManager>();
    serviceCollection.AddPluginBehavior<IStatManager, MockStatManager>();
    serviceCollection
     .AddPluginBehavior<IGangStatManager, MockInstanceStatManager>();
    serviceCollection
     .AddPluginBehavior<IPlayerStatManager, MockInstanceStatManager>();
    serviceCollection.RegisterCommands();
  }
}