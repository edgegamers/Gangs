using CounterStrikeSharp.API.Core;

namespace GangsAPI;

public interface IPluginBehavior : IBehavior {
  void IBehavior.Start() { Start(null); }
  internal void Start(BasePlugin? plugin) { }

  void Start(BasePlugin? plugin, bool hotReload) { Start(plugin); }
}