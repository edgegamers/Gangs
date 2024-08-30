using CounterStrikeSharp.API.Core;

namespace GangsAPI;

public interface IPluginBehavior : IBehavior {
  internal void Start(BasePlugin? plugin) { }

  void Start(BasePlugin? plugin, bool hotReload) { Start(plugin); }

  void IBehavior.Start() { Start(null); }
}