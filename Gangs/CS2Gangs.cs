using System.Collections.Immutable;
using CounterStrikeSharp.API.Core;
using GangsAPI;
using GangsAPI.Services.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GangsImpl;

public class CS2Gangs(IServiceProvider provider) : BasePlugin, IGangPlugin {
  private IServiceScope? scope;
  public override string ModuleName => "Gangs";
  public override string ModuleVersion => "0.0.1";
  public BasePlugin Base => this;
  public IServiceProvider Services { get; } = provider;

  public override void Load(bool hotReload) {
    scope = Services.CreateScope();
    var extensions = scope.ServiceProvider.GetServices<IPluginBehavior>()
     .ToImmutableList();

    Logger.LogInformation("[Gangs] Loading {Count} extensions",
      extensions.Count);

    foreach (var ext in extensions) {
      RegisterAllAttributes(ext);
      try {
        Logger.LogDebug("Loading {@Name}", ext.GetType().FullName);
        ext.Start(this, hotReload);
      } catch (Exception e) {
        Logger.LogError(e, "Failed to load {@Name}", ext.GetType().FullName);
      }
    }
  }
}