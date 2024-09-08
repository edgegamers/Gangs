using Commands;
using CounterStrikeSharp.API.Core;
using GangsAPI.Data;
using GangsAPI.Extensions;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Mock;
using SQLImpl;

namespace GangsImpl;

public class GangServiceCollection : IPluginServiceCollection<CS2Gangs> {
  public void ConfigureServices(IServiceCollection serviceCollection) {
    serviceCollection.AddPluginBehavior<IGangManager, MockGangManager>();
    serviceCollection.AddPluginBehavior<IPlayerManager, MockPlayerManager>();
    serviceCollection.AddPluginBehavior<IStatManager, MockStatManager>();
    serviceCollection
     .AddPluginBehavior<IGangStatManager, MockInstanceStatManager>();
    serviceCollection.AddScoped<IDBConfig, EnvDBConfig>();
    serviceCollection
     .AddPluginBehavior<IPlayerStatManager, MySQLPlayerInstanceManager>();
    serviceCollection.RegisterCommands();
  }
}