using CounterStrikeSharp.API.Core;

namespace GangsAPI;

public interface IGangPlugin : IPlugin {
  BasePlugin Base { get; }
  IServiceProvider Services { get; }
}