using Commands;
using CounterStrikeSharp.API.Core;
using GangsAPI.Extensions;
using GangsAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using Mock;
using SQLImpl;

namespace GangsImpl;

public class GangServiceCollection : IPluginServiceCollection<CS2Gangs> {
  public void ConfigureServices(IServiceCollection serviceCollection) {
    serviceCollection.AddPluginBehavior<IGangManager, MockGangManager>();
    // serviceCollection.AddPluginBehavior<IGangManager, SQLStatManager>();
    serviceCollection.AddPluginBehavior<IGangManager, SQLGangManager>();
    serviceCollection.AddPluginBehavior<IPlayerManager, MockPlayerManager>();
    serviceCollection.AddPluginBehavior<IStatManager, SQLStatManager>();
    serviceCollection
     .AddPluginBehavior<IGangStatManager, MockInstanceStatManager>();
    serviceCollection
     .AddPluginBehavior<IPlayerStatManager, MockInstanceStatManager>();
    serviceCollection.RegisterCommands();
  }
}