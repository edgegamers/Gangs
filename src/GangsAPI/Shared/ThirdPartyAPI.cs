using CounterStrikeSharp.API.Core.Capabilities;
using MAULActainShared.plugin;

namespace GangsAPI.Shared;

public static class ThirdPartyAPI {
  public static PluginCapability<IActain> ActainCapability { get; } =
    new("maulactain:core");

  public static IActain? Actain {
    get {
      try { return ActainCapability.Get(); } catch (KeyNotFoundException) {
        return null;
      }
    }
  }
}