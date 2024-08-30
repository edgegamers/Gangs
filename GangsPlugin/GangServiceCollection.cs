using CounterStrikeSharp.API.Core;
using GangsAPI;
using Microsoft.Extensions.DependencyInjection;

namespace GangsPlugin;

public class GangServiceCollection : IPluginServiceCollection<IGangPlugin> {
  public void ConfigureServices(IServiceCollection serviceCollection) { }
}