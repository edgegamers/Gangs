using Commands;
using CounterStrikeSharp.API.Core;
using GangsAPI.Data;
using GangsAPI.Extensions;
using GangsAPI.Services;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using GangsAPI.Services.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Mock;
using SQLImpl;
using Stats.Perk;
using StatsTracker;

namespace GangsImpl;

public class GangServiceCollection : IPluginServiceCollection<CS2Gangs> {
  public void ConfigureServices(IServiceCollection serviceCollection) {
    serviceCollection.AddScoped<IDBConfig, EnvDBConfig>();
    serviceCollection.AddPluginBehavior<IServerProvider, CS2ServerProvider>();
    serviceCollection.AddPluginBehavior<ITargeter, CS2Targeter>();
    serviceCollection.AddPluginBehavior<IGangManager, MySQLGangManager>();
    serviceCollection.AddPluginBehavior<IPlayerManager, MySQLPlayerManager>();
    serviceCollection
     .AddPluginBehavior<IMenuManager, CommandBasedMenuManager>();
    serviceCollection.AddPluginBehavior<IStatManager, MySQLStatManager>();
    serviceCollection
     .AddPluginBehavior<IGangStatManager, MySQLGangInstanceManager>();
    serviceCollection
     .AddPluginBehavior<IPlayerStatManager, MySQLPlayerInstanceManager>();
    serviceCollection.AddPluginBehavior<IRankManager, MySQLRankManager>();
    serviceCollection.AddPluginBehavior<IPerkManager, PerkManager>();

    serviceCollection.RegisterCommands();
    serviceCollection.RegisterStatsTracker();

    serviceCollection.AddPluginBehavior<PlayerJoinCreationListener>();
    serviceCollection
     .TryAddTransient<IStringLocalizer, PluginStringLocalizer>();
    serviceCollection.AddTransient(typeof(Lazy<>), typeof(Lazier<>));
  }

  internal class Lazier<T>(IServiceProvider provider)
    : Lazy<T>(provider.GetRequiredService<T>) where T : notnull;
}