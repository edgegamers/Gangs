using CounterStrikeSharp.API.Core;

namespace GangsAPI;

public interface IPluginBehavior : IDisposable {
  void IDisposable.Dispose() { }

  internal void Start(BasePlugin plugin) { }

  void Start(BasePlugin plugin, bool hotReload) { Start(plugin); }
}